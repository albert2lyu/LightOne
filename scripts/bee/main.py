#!/usr/bin/env python

import httplib2
import xml.etree.ElementTree as ElementTree
import pymongo
from datetime import datetime
import sys
import json
import time
import urllib.request

class YhdIndex:
	def __init__(self, http, url):
		response, content = http.request(url)
		root = ElementTree.fromstring(content)
		
		self.version = root.find('version').text
		self.updatetime = root.find('updatetime').text
		
		self.category_path = root.find('category_path').text
		self.promotion_path = root.find('promotion_path').text
		self.product_desc_path = root.find('product_desc_path').text
		self.product_descs = [node.text for node in root.find('product_descs')]
		self.product_path = root.find('product_path').text
		self.products = [node.text for node in root.find('products')]

class YhdCategories:
	def __init__(self, http, url):
		response, content = http.request(url)
		self.root = ElementTree.fromstring(content)

	def __iter__(self):
		self.iter = iter(self.root)
		return self

	def __next__(self):
		node = next(self.iter)
		return dict(cid = node.find('cid').text,
			cname = node.find('cname').text,
			pcid = node.find('pcid').text)


class YhdProducts:
	def __init__(self, http, url):
		try:
			response, content = http.request(url)
			self.root = ElementTree.fromstring(content)
		except ConnectionResetError:
			time.sleep(10)
			__init__(self, http, url)

	def __iter__(self):
		self.iter = iter(self.root)
		return self

	def parse_region_price(self, region_nodes):
		region_price_dict = {}
		for node in region_nodes:
			regions = node.attrib['region'].split(',')
			price = float(node.text)
			region_price_dict.update(dict.fromkeys(regions, price))
		return region_price_dict

	def __next__(self):
		node = next(self.iter)

		return dict(product_id = node.find('product_id').text,
			title = node.find('title').text,
			subtitle = node.find('subtitle').text,
			brand_name = node.find('brand_name').text,
			pic_url = node.find('pic_url').text,
			category_id = node.find('category_id').text,
			product_url = node.find('product_url').text,
			prices = self.parse_region_price(node.find('sale_price')),
			product_url_m = node.find('product_url_m').text)



class CategoryRepo:
	def __init__(self, db):
		self.collection = db.categories
		self.collection.ensure_index([('Source', pymongo.ASCENDING), ('Number', pymongo.ASCENDING)])

	def save(self, categories):
		sort = 0
		for category in categories:
			number = category['cid']
			name = category['cname']
			parent_number = category['pcid']
			if parent_number == '0':
				parent_number = None

			exist_category = self.collection.find_one({'$and': [{'Source': 'yhd'}, {'Number': number}]})

			if exist_category == None:
				exist_category = dict(Source = 'yhd',
					Number = number,
					CreateTime = datetime.now())
			exist_category['Name'] = name
			exist_category['ParentNumber'] = parent_number
			exist_category['UpdateTime'] = datetime.now()
			exist_category['Sort'] = sort
			sort += 1
			self.collection.save(exist_category)

	def get_category_ancestors(self, number):
		category = self.collection.find_one({'$and': [{'Source': 'yhd'}, {'Number': number}]})
		if category != None:
			if category['ParentNumber'] != None:
				for _ in self.get_category_ancestors(category['ParentNumber']):
					yield _
			yield category['_id']

class ProductRepo:
	def __init__(self, db):
		self.category_repo = CategoryRepo(db)
		self.price_history_repo = PriceHistoryRepo(db)
		self.collection = db.products
		self.collection.ensure_index([('Source', pymongo.ASCENDING), ('Number', pymongo.ASCENDING)])

	def has_changed(self, exist, new, fields):
		for field in fields:
			if exist.get(field) != new.get(field):
				return True

	@staticmethod
	def cacl_changed_ratio(oldPrice, newPrice):
		if oldPrice == None or newPrice == None or oldPrice == 0:
			return 0
		return (newPrice - oldPrice) / oldPrice

	def save(self, products):
		for product in products:
			new_product = dict(
				Number = product['product_id'],
				Name = product['title'],
				SubTitle = product['subtitle'],
				Brand = product['brand_name'],
				ImgUrl = product['pic_url'],
				CategoryIds = list(self.category_repo.get_category_ancestors(product['category_id'])),
				Url = product['product_url']
				)
			if '北京' in product['prices']:
				new_product['Price'] = product['prices']['北京']

			# url_m = product['product_url_m']
			exist_product = self.collection.find_one({'$and': [{'Source': 'yhd'}, {'Number': new_product['Number']}]})
			if exist_product == None:
				exist_product = dict(Source = 'yhd',
					Number = new_product['Number'],
					CreateTime = datetime.now())

			has_changed = self.has_changed(exist_product, new_product, ('Number', 'Name', 'SubTitle', 'Brand', 'ImgUrl', 'CategoryIds', 'Url', 'Price'))
			if has_changed:
				# 处理价格变化
				old_price = exist_product.get('Price')
				new_price = new_product.get('Price')
				if old_price != new_price:
					# 价格发生变化
					exist_product['OldPrice'] = old_price
					exist_product['ChangedRatio'] = ProductRepo.cacl_changed_ratio(old_price, new_price)

				exist_product.update(new_product)
				exist_product['UpdateTime'] = datetime.now()
				self.collection.save(exist_product)

				# 记录历史价格
				if old_price != new_price:
					self.price_history_repo.save(exist_product['_id'], new_price)

		print('.', end = '', flush = True)

	def get_all(self):
		#return self.collection.find(sort=[('UpdateTime', pymongo.ASCENDING)])
		return self.collection.find()

class PriceHistoryRepo:
	def __init__(self, db):
		self.collection = db.price_history

	def save(self, product_id, price):
		self.collection.find_and_modify({'_id': product_id},
			{ '$push': { '_': {'time': datetime.now(), 'price': price }}},
			upsert = True)


def extract_products(h, db):
	index = YhdIndex(h, 'http://union.yihaodian.com/api/productInfo/yihaodian/index.xml')

	print('处理分类')
	categories = YhdCategories(h, index.category_path)
	CategoryRepo(db).save(categories)

	print('处理产品')
	for products_url in index.products:
		products = YhdProducts(h, index.product_path + products_url)
		ProductRepo(db).save(products)

def extract_price(h, db):
	all_products = ProductRepo(db).get_all()

	count = 0
	products = list()
	for product in all_products:
		products.append(product)
		if len(products) == 20:
			process_products_price(h, db, products)
			products.clear()

		count += 1
		if count % 1000 == 0:
			print('.', end = '', flush = True)

	# 处理不足一批的products
	if len(products) > 0:
		process_products_price(h, db, products)

def process_products_price(http, db, products):
	price_history_repo = PriceHistoryRepo(db)

	try:
		url = 'http://busystock.i.yihaodian.com/busystock/restful/truestock?mcsite=1&provinceId=2&' + '&'.join('productIds=' + p['Number'] for p in products)
		response = urllib.request.urlopen(url)
		content = response.read()
		# response, content = http.request(url)
		for item in json.loads(content.decode("utf-8")):
			for product in products:
				if product['Number'] == str(item['productId']):
					old_price = product.get('Price')
					new_price = item['productPrice']
					if (old_price != new_price):
						product['OldPrice'] = old_price
						product['Price'] = new_price
						product['ChangedRatio'] = ProductRepo.cacl_changed_ratio(old_price, new_price)
						product['UpdateTime'] = datetime.now()
						db.products.collection.save(product)
						price_history_repo.save(product['_id'], new_price)
					break
	except:
		print("Unexpected error:", sys.exc_info()[0])
		time.sleep(60)
		process_products_price(http, db, products)

if __name__ == '__main__':
	h = httplib2.Http('.cache')
	db = pymongo.MongoClient().queen_new

	if len(sys.argv) == 2:
		extract_products(h, db)
	else:
		extract_price(h, db)

	print('done')
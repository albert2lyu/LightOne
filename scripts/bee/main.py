#!/usr/bin/env python

import httplib2
import xml.etree.ElementTree as ElementTree
import pymongo
from datetime import datetime

class Index:
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

class Categories:
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



class Products:
	def __init__(self, http, url):
		response, content = http.request(url)
		self.root = ElementTree.fromstring(content)

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

class Database:
	def __init__(self, db):
		self.db = db

	def save_categories(self, categories):
		collection = self.db.categories
		collection.ensure_index([('Source', pymongo.ASCENDING), ('Number', pymongo.ASCENDING)])

		sort = 0
		for category in categories:
			number = category['cid']
			name = category['cname']
			parent_number = category['pcid']
			if parent_number == '0':
				parent_number = None

			exist_category = collection.find_one({'$and': [{'Source': 'yhd'}, {'Number': number}]})

			if exist_category == None:
				exist_category = dict(Source = 'yhd',
					Number = number,
					CreateTime = datetime.now())
			exist_category['Name'] = name
			exist_category['ParentNumber'] = parent_number
			exist_category['UpdateTime'] = datetime.now()
			exist_category['Sort'] = sort
			sort += 1
			collection.save(exist_category)

	def has_changed(self, exist, new, fields):
		for field in fields:
			if exist.get(field) != new.get(field):
				return True

	def cacl_changed_ratio(self, oldPrice, newPrice):
		if oldPrice == None or newPrice == None or oldPrice == 0:
			return 0
		return (newPrice - oldPrice) / oldPrice

	def save_products(self, products):
		collection = self.db.products
		collection.ensure_index([('Source', pymongo.ASCENDING), ('Number', pymongo.ASCENDING)])

		for product in products:
			new_product = dict(
				Number = product['product_id'],
				Name = product['title'],
				SubTitle = product['subtitle'],
				Brand = product['brand_name'],
				ImgUrl = product['pic_url'],
				CategoryIds = list(self.get_category_ancestors(product['category_id'])),
				Url = product['product_url']
				)
			if '北京' in product['prices']:
				new_product['Price'] = product['prices']['北京']

			# url_m = product['product_url_m']
			exist_product = collection.find_one({'$and': [{'Source': 'yhd'}, {'Number': new_product['Number']}]})
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
					exist_product['ChangedRatio'] = self.cacl_changed_ratio(old_price, new_price)

				exist_product.update(new_product)
				exist_product['UpdateTime'] = datetime.now()
				collection.save(exist_product)

				# 记录历史价格
				if old_price != new_price:
					self.save_price_history(exist_product['_id'], new_price)

		print('.', end = '', flush = True)

	def save_price_history(self, product_id, price):
		collection = self.db.price_history
		collection.find_and_modify({'_id': product_id},
			{ '$push': { '_': {'time': datetime.now(), 'price': price }}},
			upsert = True)

	def get_category_ancestors(self, number):
		collection = self.db.categories
		collection.ensure_index([('Source', pymongo.ASCENDING), ('Number', pymongo.ASCENDING)])

		category = collection.find_one({'$and': [{'Source': 'yhd'}, {'Number': number}]})
		if category != None:
			if category['ParentNumber'] != None:
				for _ in self.get_category_ancestors(category['ParentNumber']):
					yield _
			yield category['_id']





if __name__ == '__main__':
	h = httplib2.Http('.cache')
	index = Index(h, 'http://union.yihaodian.com/api/productInfo/yihaodian/index.xml')

	db = Database(pymongo.MongoClient().queen_new)

	print('处理分类')
	categories = Categories(h, index.category_path)
	db.save_categories(categories)
	print('done')

	print('处理产品')
	for products_url in index.products:
		products = Products(h, index.product_path + products_url)
		db.save_products(products)
	print('done')

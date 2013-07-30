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
			prices = self.parse_region_price(node.find('sale_price'))
			#product_url_m = node.find('product_url_m').text,
			#prices = _['region'] for _ in node.find('sale_price')
			)

def save_categories(db, categories):
	collection = db.categories
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

def save_products(db, products):
	collection = db.products
	collection.ensure_index([('Source', pymongo.ASCENDING), ('Number', pymongo.ASCENDING)])

	for product in products:
		number = product['product_id']
		name = product['title']
		subtitle = product['subtitle']
		brand = product['brand_name']
		img_url = product['pic_url']
		category_number = product['category_id']
		url = product['product_url']
		prices = product['prices']
		print(prices)
		#url_m = product['product_url_m']
		# price
		# oldprice
		# changedratio
		# pricehistory
		exist_product = collection.find_one({'$and': [{'Source': 'yhd'}, {'Number': number}]})
		if exist_product == None:
			exist_product = dict(Source = 'yhd',
				Number = number,
				CreateTime = datetime.now())

		exist_product['UpdateTime'] = datetime.now()
		exist_product['CategoryIds'] = list(get_category_ancestors(db, category_number))
		exist_product['Name'] = name
		exist_product['SubTitle'] = subtitle
		exist_product['Brand'] = brand
		exist_product['ImgUrl'] = img_url
		exist_product['Url'] = url
		exist_product['Price'] = prices['北京']
		#exist_product['Url'] = url_m
		collection.save(exist_product)

	print('.', end = '', flush = True)

def get_category_ancestors(db, number):
	collection = db.categories
	collection.ensure_index([('Source', pymongo.ASCENDING), ('Number', pymongo.ASCENDING)])

	category = collection.find_one({'$and': [{'Source': 'yhd'}, {'Number': number}]})
	if category != None:
		if category['ParentNumber'] != None:
			for _ in get_category_ancestors(db, category['ParentNumber']):
				yield _
		yield category['_id']





if __name__ == '__main__':
	h = httplib2.Http('.cache')
	index = Index(h, 'http://union.yihaodian.com/api/productInfo/yihaodian/index.xml')

	db = pymongo.MongoClient().queen_new

	print('处理分类')
	categories = Categories(h, index.category_path)
	save_categories(db, categories)
	print('done')

	print('处理产品')
	for products_url in index.products:
		products = Products(h, index.product_path + products_url)
		save_products(db, products)
		break
	print('done')

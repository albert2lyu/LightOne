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

	def __next__(self):
		node = next(self.iter)
		return dict(product_id = node.find('product_id').text,
			title = node.find('title').text,
			subtitle = node.find('subtitle').text,
			brand_name = node.find('brand_name').text,
			pic_url = node.find('pic_url').text,
			category_id = node.find('category_id').text,
			product_url = node.find('product_url').text,
			product_url_m = node.find('product_url_m').text)

def save_categories(db, categories):
	category_collection = db.categories
	category_collection.ensure_index([('Source', pymongo.ASCENDING), ('Number', pymongo.ASCENDING)])

	sort = 0
	for category in categories:
		number = category['cid']
		name = category['cname']
		parent_number = category['pcid']
		if parent_number == '0':
			parent_number = None

		exist_category = category_collection.find_one({'$and': [{'Source': 'yhd'}, {'Number': number}]})

		if exist_category == None:
			exist_category = dict(Source = 'yhd',
				Number = number,
				CreateTime = datetime.now())
		exist_category['Name'] = name
		exist_category['ParentNumber'] = parent_number
		exist_category['UpdateTime'] = datetime.now()
		exist_category['Sort'] = sort
		sort += 1
		category_collection.save(exist_category)

	print('分类分析完毕')

if __name__ == '__main__':
	h = httplib2.Http('.cache')
	index = Index(h, 'http://union.yihaodian.com/api/productInfo/yihaodian/index.xml')
	# categories = Categories(h, index.category_path)

	# db = pymongo.MongoClient().queen_new
	# save_categories(db, categories)

	for products_url in index.products:
		products = Products(h, index.product_path + products_url)
		for p in products: print(p)
		break

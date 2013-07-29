#!/usr/bin/env python

import httplib2
import xml.etree.ElementTree as ElementTree
from pymongo import MongoClient

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


if __name__ == '__main__':
	h = httplib2.Http('.cache')
	index = Index(h, 'http://union.yihaodian.com/api/productInfo/yihaodian/index.xml')
	categories = Categories(h, index.category_path)
	print(list(categories)[:10])

	db = MongoClient().queen
	category_collection = db.categories
	
	print(category_collection)


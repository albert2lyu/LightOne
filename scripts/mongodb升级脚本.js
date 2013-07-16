// 升级ratio_rankings表
db.ratio_rankings.find().forEach(function(o) {
    o.CategoryId = new ObjectId(o.CategoryId);
    if (o.CategoryId == null)
    	o.CategoryId = new ObjectId("000000000000000000000000");
    var newProductIds = [];
    o.ProductIds.forEach(function(p){
        newProductIds.push(new ObjectId(p));
    });
    o.ProductIds = newProductIds;
    db.ratio_rankings.save(o);
})

// 升级products表
var count = 0;
db.products.find({"CategoryIds.0": { $type : 2 }}).forEach(function(o) {
    var newCategoryIds = [];
    o.CategoryIds.forEach(function(c){
        newCategoryIds.push(new ObjectId(c));
    });
    o.CategoryIds = newCategoryIds;
    
    o.Price = parseFloat(o.Price);
    o.OldPrice = parseFloat(o.OldPrice);
    o.ChangedRatio = parseFloat(o.ChangedRatio);

    // o.PriceHistory.forEach(function(h){
    // 	h.Price = parseFloat(h.Price);
    // });
    
    db.products.save(o);

    if (count++ % 1000 == 0)
    	print(count);
});

var count = 0;
db.products.find({"PriceHistory.0.Price": { $type : 2 }}).forEach(function(o) {
    o.PriceHistory.forEach(function(h){
    	h.Price = parseFloat(h.Price);
    });
    
    db.products.save(o);

    if (count++ % 1000 == 0)
    	print(count);
});


db.products.find({"Price": { $type : 2 }}).count();
db.products.find({"OldPrice": { $type : 2 }}).count();
db.products.find({"ChangedRatio": { $type : 2 }}).count();
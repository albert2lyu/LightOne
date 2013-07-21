using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Business {
    public class CategoryArchiveService {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CategoryRepo _CategoryRepo = new CategoryRepo();

        /// <summary>
        /// 保存分类信息，返回需要抓取数据的分类
        /// </summary>
        /// <param name="categories"></param>
        public IEnumerable<Category> Archive(IList<Category> categories) {
            //var cat = categories.FirstOrDefault();
            //if (cat == null)
            //    yield break;

            //var existsCategories = _CategoryRepo.GetBySource(cat.Source).ToList();

            foreach (var category in categories) {
                var existsCategory = _CategoryRepo.GetBySourceAndNumber(category.Source, category.Number);
                //var existsCategory = existsCategories.FirstOrDefault(ec => ec.Source == category.Source && ec.Number == category.Number);

                if (existsCategory == null) {
                    // 新分类
                    category.CreateTime = DateTime.Now;
                    category.ProductsUpdateTime = DateTime.Now;
                }
                else {
                    // 已经存在的分类
                    category.Id = existsCategory.Id;
                    category.CreateTime = existsCategory.CreateTime;
                    category.StableTimes = existsCategory.StableTimes;
                    category.ProductsUpdateTime = existsCategory.ProductsUpdateTime;
                }
                category.UpdateTime = DateTime.Now;
                _CategoryRepo.Save(category);

                // 返回需要抓取数据的分类
                if (NeedReExtract(existsCategory))
                    yield return category;
            }

            // 禁用已经不存在的分类，不用这么做，只需要定期Disable updateTime很久以前的分类即可
            //var notExistsCategories = existsCategories.Except(categories, new CategoryEqualityComparer());
            //foreach (var category in notExistsCategories) {
            //    _CategoryRepo.DisableById(category.Id);
            //}
        }

        public bool NeedReExtract(Category existsCategory) {
            return existsCategory == null || (
                existsCategory.Level == 3 &&
                existsCategory.ProductsUpdateTime + TimeSpan.FromHours(existsCategory.StableTimes) < DateTime.Now
                );
            //&& GetAncestorCategories(Id).All(c => c.Number != "123" && c.Number != "25228");
        }
    }
}

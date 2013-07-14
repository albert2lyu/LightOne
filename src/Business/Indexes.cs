using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business {
    public class Indexes {
        public static void Ensure(){
            
            ProductRepo.Collection.EnsureIndex(IndexKeys.Ascending("CategoryIds"));
            ProductRepo.Collection.EnsureIndex(IndexKeys.Ascending("UpdateTime"));
            ProductRepo.Collection.EnsureIndex(IndexKeys.Ascending("Source", "Number"), IndexOptions.SetUnique(true));
            ProductRepo.Collection.EnsureIndex(IndexKeys.Ascending("ChangedRatio", "UpdateTime", "CategoryIds"));

            CategoryRepo.Collection.EnsureIndex(IndexKeys.Ascending("Source", "Number"), IndexOptions.SetUnique(true));

            RatioRankingRepo.Collection.EnsureIndex(IndexKeys.Ascending("CategoryId"), IndexOptions.SetUnique(true));
        }
    }
}

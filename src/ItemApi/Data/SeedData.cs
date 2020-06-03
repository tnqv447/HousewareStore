using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItemApi.Models;

namespace ItemApi.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(ItemContext context)
        {
            context.Database.EnsureCreated();

            //seed categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category> {
                    new Category {
                    //CategoryId = 0,
                    CategoryName = "Nồi, chảo"
                    },
                    new Category {
                    //CategoryId = 1,
                    CategoryName = "Đồ dùng nhà bếp"
                    },
                    new Category {
                    //CategoryId = 2,
                    CategoryName = "Tô, chén, đĩa"
                    },
                    new Category {
                    //CategoryId = 3,
                    CategoryName = "Ly, lọ, bình"
                    },
                    new Category {
                    //CategoryId = 4,
                    CategoryName = "Đồ dùng phòng khách"
                    },
                    new Category {
                    //CategoryId = 5,
                    CategoryName = "Đồ dùng vệ sinh"
                    },
                    new Category {
                    //CategoryId = 6,
                    CategoryName = "Đồ dùng khác"
                    }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            //seed items
            if (!context.Items.Any())
            {
                var items = new List<Item> {
                    new Item {
                    //Id = 2,
                    Name = "Bộ 5 nồi sứ cao cấp",
                    UnitPrice = 250000,
                    Description = "Sứ chất lượng kao.",
                    PictureUrl = "",
                    ItemStatus = ItemStatus.Approved,
                    DbStatus = DbStatus.Active,
                    CategoryId = 1
                    },
                    new Item {
                    //Id = 0,
                    Name = "Muỗng basic",
                    UnitPrice = 20000,
                    Description = "Không muỗng mà muốn ăn thì chỉ có ăn dơ ăn bốc.",
                    PictureUrl = "",
                    ItemStatus = ItemStatus.Approved,
                    DbStatus = DbStatus.Active,
                    CategoryId = 2
                    },
                    new Item {
                    //Id = 1,
                    Name = "Đũa basic",
                    UnitPrice = 15000,
                    Description = "Không đũa mà cũng muốn ăn thì cũng chỉ có ăn dơ ăn bốc.",
                    PictureUrl = "",
                    ItemStatus = ItemStatus.Approved,
                    DbStatus = DbStatus.Active,
                    CategoryId = 2
                    }

                };
                await context.Items.AddRangeAsync(items);
                await context.SaveChangesAsync();

                // var temp = context.Items.ElementAt(1);
                // context.Items.Remove(temp);
                // await context.SaveChangesAsync();

                // context.Items.Add(new Item
                // {
                //     //Id = 0,
                //     Name = "Muỗng temp",
                //     Price = 20000,
                //     Description = "Không muỗng mà muốn ăn thì chỉ có ăn dơ ăn bốc.",
                //     PictureUrl = "",
                //     Status = ItemStatus.Approved,
                //     CategoryId = 2
                // });

                // await context.SaveChangesAsync();
            }
        }
    }
}
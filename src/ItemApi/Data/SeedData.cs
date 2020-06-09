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
                        //Id = 10,
                        Name = "Ấm Siêu Tốc Nagakawa",
                        UnitPrice = 109000,
                        Description = "Dung tích lớn: 1.8 lít. Công suất mạnh mẽ: 1500W. Tự ngắt điện khi nước sôi",
                        PictureUrl = "item_10.jpg",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 2
                    },
                    new Item {
                        //Id = 9,
                        Name = "Bình Giữ Nhiệt Inox 1200ml",
                        UnitPrice = 120000,
                        Description = "Dung tích bình lớn tiện lợi cho việc sử dụng lâu dài trong 1 ngày.",
                        PictureUrl = "item_9.jpg",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 4
                    },
                    new Item {
                        //Id = 8,
                        Name = "Bộ 12 Ly Thủy Tinh Có Quai",
                        UnitPrice = 119000,
                        Description = "Dung tích: 375ml; Kích thước ly: 8,33 cm x 11,9 cm. \nThiết kế tinh tế, sang trọng, mang tính thẩm mỹ cao.",
                        PictureUrl = "item_8.jpg",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 4
                    },
                    new Item {
                        //Id = 7,
                        Name = "Chảo đá chống dính",
                        UnitPrice = 90000,
                        Description = "Tráng men đá chống dính, bong tróc. \nTay cầm làm bằng nhựa chống cháy ở nhiệt độ cao. \nPhù hợp sử dụng chiên xào nhiều món",
                        PictureUrl = "item_7.jpg",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 1
                    },
                    new Item {
                        //Id = 6,
                        Name = "Chảo Chống Dính Bếp Từ Fivestar",
                        UnitPrice = 149000,
                        Description = "Được làm từ inox không gỉ. \nChảo chống dính thiết kế 3 đáy. \nbề mặt chảo sần caro khi nấu ăn dầu không dính nhiều trên thực phẩm",
                        PictureUrl = "item_6.jpg",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 1
                    },
                    new Item {
                        //Id = 5,
                        Name = "Lò Nướng Điện Sunhouse",
                        UnitPrice = 369000,
                        Description = "Công nghệ nướng Halogen tiết kiệm điện năng. \nThanh nướng kép – Truyền nhiệt nhanh, nướng chín đều. \nHẹn giờ & Điều chỉnh nhiệt độ linh hoạt. \nGiá để thực phẩm bằng thép không gỉ, an toàn cho sức khỏe. \nĐiều khiển bằng núm xoay dễ dàng, tiện lợi",
                        PictureUrl = "item_5.jpg",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 2
                    },
                    new Item {
                        //Id = 4,
                        Name = "Nồi Cơm Điện Tử Toshiba",
                        UnitPrice = 1699000,
                        Description = "Có 3 mâm nhiệt, làm cơm chín đều. Thiết kế quai xách tiện dụng. Nhiều chế độ nấu ăn đa dạng",
                        PictureUrl = "item_4.jpg",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 2
                    },
                    new Item
                    {
                        //Id = 3,
                        Name = "Bộ 5 nồi sứ cao cấp",
                        UnitPrice = 250000,
                        Description = "Sứ chất lượng kao.",
                        PictureUrl = "item_3.jpg",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 3
                    },
                    new Item
                    {
                        //Id = 2,
                        Name = "Muỗng basic",
                        UnitPrice = 20000,
                        Description = "Không muỗng mà muốn ăn thì chỉ có ăn dơ ăn bốc.",
                        PictureUrl = "item_2.jpg",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 2
                    },
                    new Item
                    {
                        //Id = 1,
                        Name = "Đũa basic",
                        UnitPrice = 15000,
                        Description = "Không đũa mà cũng muốn ăn thì cũng chỉ có ăn dơ ăn bốc.",
                        PictureUrl = "item_1.jpg",
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
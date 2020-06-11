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
                    CategoryName = "Pots & Pans"
                    },
                    new Category {
                    //CategoryId = 1,
                    CategoryName = "Kitchen Utensils"
                    },
                    new Category {
                    //CategoryId = 2,
                    CategoryName = "Bowls, Cups, Forks"
                    },
                    new Category {
                    //CategoryId = 3,
                    CategoryName = "Cups, Jars, Bottles"
                    },
                    new Category {
                    //CategoryId = 4,
                    CategoryName = "Living Room Furniture"
                    },
                    new Category {
                    //CategoryId = 5,
                    CategoryName = "Toiletries"
                    },
                    new Category {
                    //CategoryId = 6,
                    CategoryName = "Other utensils"
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
                        Name = "OXO Good Grips 2-Cup Angled Measuring Cup",
                        UnitPrice = 8.99,
                        Description = "Patented angled surface lets you read measurements from above. Eliminates the need to fill, check and adjust. Soft, non slip handle",
                        PictureUrl = "item_10.jpg",
                        OwnerId = "b682f90b-1157-4c36-bdf3-5099cbab02a7",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 2
                    },
                    new Item {
                        //Id = 9,
                        Name = "Bormioli Rocco Fido Square Clear Jar with Chalkboard, 50-3/4-Ounce",
                        UnitPrice = 20.83,
                        Description = "Chalk label is prefect for writing contents; wipe clean with a damp cloth and reuse again and again. Lid has an airtight seal to keep food fresh. Versatile jar; can be used to store and display sugar; flour; tea; candy and much more",
                        PictureUrl = "item_9.jpg",
                        OwnerId = "b682f90b-1157-4c36-bdf3-5099cbab02a7",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 4
                    },
                    new Item {
                        //Id = 8,
                        Name = "MITBAK 13- OZ Colored Highball Glasses (Set of 6)",
                        UnitPrice = 39.99,
                        Description = " ELEGANT GLASSES WITH A TOUCH OF FLAIR – These highball glasses have a sharp, classic shape combined with a modern, colorful twist. The Collins glasses are carefully crafted from lead-free crystal and come six to a set. MADE IN SLOVAKIA. FIRM BASE & THIN SHAPE FOR AN EASY GRIP",
                        PictureUrl = "item_8.jpg",
                        OwnerId = "b682f90b-1157-4c36-bdf3-5099cbab02a7",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 4
                    },
                    new Item {
                        //Id = 7,
                        Name = "OXO Good Grips 11-Inch Better Balloon Whisk",
                        UnitPrice = 9.93,
                        Description = "Polished stainless steel wires and narrow shape are perfect for whisking in a small bowl or container. \nInnovative handle shape and soft, comfortable grip to absorb pressure. Dishwasher safe",
                        PictureUrl = "item_7.jpg",
                        OwnerId = "8ce6c08a-207b-48a4-85e2-741ad4041cd5",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 2
                    },
                    new Item {
                        //Id = 6,
                        Name = "AmazonBasics Non-Stick Cookware Set, Pots and Pans - 8-Piece Set",
                        UnitPrice = 41.49,
                        Description = "8-piece nonstick cookware set includes 8-inch fry pan, 10-inch fry pan, 1.5-quart sauce pan with lid, 2 quart saucepan with lid, and 3-quart casserole pan with lid. Aluminum body with non-stick coating for easy cooking and cleaning--BPA-free. Comfortable soft-touch handles stay cool during use, vented glass lids let steam escape",
                        PictureUrl = "item_6.jpg",
                        OwnerId = "6cbb393b-2043-4330-998f-032d5f0ea957",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 1
                    },
                    new Item {
                        //Id = 5,
                        Name = "Toaster Oven",
                        UnitPrice = 49.97,
                        Description = "Premium Quality Components. Imported. Top Rated Quality!",
                        PictureUrl = "item_5.jpg",
                        OwnerId = "6cbb393b-2043-4330-998f-032d5f0ea957",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 2
                    },
                    new Item {
                        //Id = 4,
                        Name = "Instant Pot Duo 7-in-1 Electric Pressure Cooker, Slow Cooker, Rice Cooker, Steamer, Saute, Yogurt Maker, and Warmer, 6 Quart, 14 One-Touch Programs",
                        UnitPrice = 79.00,
                        Description = "Best Selling Model: America’s most loved multi cooker, built with the latest 3rd generation technology, the microprocessor monitors pressure, temperature, keeps time, and adjusts heating intensity and duration to achieve desired results every time. Cooks Fast and Saves Time: The Instant Pot Duo multi-cooker combines 7 appliances in one: pressure cooker, slow cooker, rice cooker, steamer, sauté pan, yogurt maker and warmer – and cooks up to 70% faster. Consistently Delicious: 14 one-touch Smart Programs put cooking ribs, soups, beans, rice, poultry, yogurt, desserts and more on autopilot – with tasty results",
                        PictureUrl = "item_4.jpg",
                        OwnerId = "6cbb393b-2043-4330-998f-032d5f0ea957",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 1
                    },
                    new Item
                    {
                        //Id = 3,
                        Name = "Home Hero Kitchen Utensil Set - 23 Nylon Cooking Utensils - Kitchen Utensils with Spatula - Kitchen Gadgets Cookware Set - Best Kitchen Tool Set",
                        UnitPrice = 24.99,
                        Description = "23 NYLON COOKING UTENSILS - Utensil Set includes everything you need to cook that perfect dish. HIGH QUALITY THAT LASTS - These kitchen gadgets are made to stand the test of time. Top of the range 430 stainless steel handles provide greater durability than other plastic kitchen utensil sets. PERFECT FOR NON-STICK - Nylon heads prevent these kitchen tools from scratching and damaging expensive non-stick pots and pans. BEST VALUE FOR MONEY AROUND - this fully comprehensive high quality utensil set is available at an incredibly low price",
                        PictureUrl = "item_3.jpg",
                        OwnerId = "8ce6c08a-207b-48a4-85e2-741ad4041cd5",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 2
                    },
                    new Item
                    {
                        //Id = 2,
                        Name = "Wilton Icing Spatula, 13-Inch, Angled Cake Spatula",
                        UnitPrice = 3.84,
                        Description = "Angled spatula is great for frosting and smoothing icing on cakes without getting your fingers in the icing. Ergonomic handle is shaped for comfort and control. Material: Steel blade, plastic handle. 13 inches (33 centimeter).",
                        PictureUrl = "item_2.jpg",
                        OwnerId = "8ce6c08a-207b-48a4-85e2-741ad4041cd5",
                        ItemStatus = ItemStatus.Approved,
                        DbStatus = DbStatus.Active,
                        CategoryId = 2
                    },
                    new Item
                    {
                        //Id = 1,
                        Name = "OXO Spatulas, Small, BLACK",
                        UnitPrice = 9.99,
                        Description = "A necessary tool when cooking with non-stick cookware and bakeware; will not scratch pots and pans. Has a thin, flexible edge that glides easily beneath foods; perfect for flipping fragile foods and keeping them intact. Combines the strength of stainless steel with the advantages of silicone; outer silicone layer bonded to flexible stainless-steel core",
                        PictureUrl = "item_1.jpg",
                        OwnerId = "8ce6c08a-207b-48a4-85e2-741ad4041cd5",
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
using Microsoft.EntityFrameworkCore;
using MyStore.Api.Models.Entities;

namespace MyStore.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Users.AnyAsync())
        {
            db.Users.Add(new User
            {
                FullName = "Admin",
                Email = "admin@mystore.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin#123"),
                Role = "Admin"
            });
            await db.SaveChangesAsync();
        }

        if (await db.Categories.AnyAsync()) return;

        var iphones = new Category { Name = "iPhone", Slug = "iphone", Description = "Latest iPhones — Pro, Pro Max, and standard models." };
        var macs    = new Category { Name = "MacBook", Slug = "macbook", Description = "MacBook Air and MacBook Pro powered by Apple silicon." };
        var ipads   = new Category { Name = "iPad", Slug = "ipad", Description = "iPad, iPad Air, iPad mini, and iPad Pro." };
        var accs    = new Category { Name = "Accessories", Slug = "accessories", Description = "AirPods, Apple Watch, chargers, and more." };

        db.Categories.AddRange(iphones, macs, ipads, accs);
        await db.SaveChangesAsync();

        var products = new List<Product>
        {
            new() {
                Name = "iPhone 15 Pro Max 256GB",
                Description = "6.7\" Super Retina XDR display, A17 Pro chip, titanium design, 5x telephoto camera.",
                Price = 1199m, Stock = 25, Brand = "Apple", IsFeatured = true, CategoryId = iphones.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-15-pro-finish-select-202309-6-7inch-naturaltitanium?wid=2560&hei=1440&fmt=p-jpg&qlt=80&.v=1693342290295"
            },
            new() {
                Name = "iPhone 15 Pro 128GB",
                Description = "6.1\" Super Retina XDR, A17 Pro, titanium frame, USB-C, Action Button.",
                Price = 999m, Stock = 30, Brand = "Apple", IsFeatured = true, CategoryId = iphones.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-15-pro-finish-select-202309-6-1inch-bluetitanium?wid=2560&hei=1440&fmt=p-jpg&qlt=80&.v=1692923776595"
            },
            new() {
                Name = "iPhone 15 128GB",
                Description = "6.1\" display with Dynamic Island, A16 Bionic, 48MP main camera, USB-C.",
                Price = 799m, Stock = 60, Brand = "Apple", IsFeatured = true, CategoryId = iphones.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-15-finish-select-202309-6-1inch-pink?wid=2560&hei=1440&fmt=p-jpg&qlt=80&.v=1692924188181"
            },
            new() {
                Name = "iPhone 14 128GB",
                Description = "6.1\" Super Retina XDR display, A15 Bionic, dual-camera system, Crash Detection.",
                Price = 699m, Stock = 40, Brand = "Apple", IsFeatured = false, CategoryId = iphones.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-14-finish-select-202209-6-1inch-blue?wid=2560&hei=1440&fmt=p-jpg&qlt=80&.v=1660753093656"
            },

            new() {
                Name = "MacBook Pro 14\" M3 Pro",
                Description = "14.2\" Liquid Retina XDR display, M3 Pro chip, 18GB RAM, 512GB SSD.",
                Price = 1999m, Stock = 15, Brand = "Apple", IsFeatured = true, CategoryId = macs.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/mbp14-spaceblack-select-202310?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1697311054290"
            },
            new() {
                Name = "MacBook Air 13\" M2",
                Description = "13.6\" Liquid Retina display, Apple M2 chip, 8GB unified memory, 256GB SSD.",
                Price = 1099m, Stock = 35, Brand = "Apple", IsFeatured = true, CategoryId = macs.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/mba13-midnight-select-202402?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1707414914194"
            },
            new() {
                Name = "MacBook Air 15\" M3",
                Description = "15.3\" Liquid Retina display, M3 chip, 8GB unified memory, 256GB SSD.",
                Price = 1299m, Stock = 22, Brand = "Apple", IsFeatured = false, CategoryId = macs.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/mba15-starlight-select-202402?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1707414985464"
            },

            new() {
                Name = "iPad Pro 11\" M4",
                Description = "Ultra Retina XDR display, M4 chip, Apple Pencil Pro support, 256GB.",
                Price = 999m, Stock = 18, Brand = "Apple", IsFeatured = true, CategoryId = ipads.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/ipad-pro-13-select-cell-spaceblack-202405?wid=2560&hei=1440&fmt=p-jpg&qlt=80&.v=1713923480112"
            },
            new() {
                Name = "iPad Air 11\" M2",
                Description = "11\" Liquid Retina display, M2 chip, USB-C, 128GB.",
                Price = 599m, Stock = 28, Brand = "Apple", IsFeatured = false, CategoryId = ipads.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/ipad-air-11-select-wifi-blue-202405?wid=2560&hei=1440&fmt=p-jpg&qlt=80&.v=1713308179770"
            },
            new() {
                Name = "iPad mini",
                Description = "8.3\" Liquid Retina display, A15 Bionic, Touch ID, 64GB.",
                Price = 499m, Stock = 30, Brand = "Apple", IsFeatured = false, CategoryId = ipads.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/ipad-mini-select-wifi-purple-202109?wid=940&hei=1112&fmt=png-alpha&.v=1631661775000"
            },

            new() {
                Name = "AirPods Pro (2nd gen)",
                Description = "Active Noise Cancellation, Adaptive Audio, USB-C charging case.",
                Price = 249m, Stock = 80, Brand = "Apple", IsFeatured = true, CategoryId = accs.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MTJV3?wid=572&hei=572&fmt=jpeg&qlt=95&.v=1694014871985"
            },
            new() {
                Name = "Apple Watch Series 9 45mm",
                Description = "Always-On Retina display, S9 SiP, Double Tap gesture, GPS.",
                Price = 429m, Stock = 45, Brand = "Apple", IsFeatured = true, CategoryId = accs.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MR9C3ref_VW_34FR+watch-45-alum-midnight-nc-s9_VW_34FR+watch-face-45-aluminum-midnight-s9_VW_34FR?wid=1400&hei=1400&trim=1%2C0&fmt=p-jpg&qlt=95&.v=1694507905569"
            },
            new() {
                Name = "Magic Keyboard for iPad Pro",
                Description = "Built-in trackpad, USB-C pass-through charging, floating cantilever design.",
                Price = 299m, Stock = 25, Brand = "Apple", IsFeatured = false, CategoryId = accs.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MJQJ3?wid=572&hei=572&fmt=jpeg&qlt=95&.v=1617126613000"
            },
            new() {
                Name = "Apple Pencil Pro",
                Description = "Squeeze gestures, Find My, barrel roll, haptic feedback.",
                Price = 129m, Stock = 60, Brand = "Apple", IsFeatured = false, CategoryId = accs.Id,
                ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MX2D3?wid=572&hei=572&fmt=jpeg&qlt=95&.v=1713380790067"
            }
        };
        db.Products.AddRange(products);
        await db.SaveChangesAsync();
    }
}

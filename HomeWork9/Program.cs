using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HomeWork9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ApplicationDbContext())
            {
                var productService = new ProductService(context);

                var newProduct = new Product
                {
                    Name = "Test Product",
                    Description = "This is a test product",
                    Price = 19.99m,
                    AvailableQuantity = 100
                };

                productService.AddProduct(newProduct);

                var products = productService.GetProducts();
                Console.WriteLine("List of products:");
                foreach (var product in products)
                {
                    Console.WriteLine($"{product.Id} - {product.Name} - {product.Price:C} - {product.AvailableQuantity} in stock");
                }

                var productToUpdate = products.FirstOrDefault();
                if (productToUpdate != null)
                {
                    Console.WriteLine($"Updating product with Id {productToUpdate.Id}");
                    productToUpdate.Price = 24.99m;
                    productToUpdate.AvailableQuantity = 80;

                    productService.UpdateProduct(productToUpdate);
                }

                var updatedProducts = productService.GetProducts();
                Console.WriteLine("List of updated products:");
                foreach (var updatedProduct in updatedProducts)
                {
                    Console.WriteLine($"{updatedProduct.Id} - {updatedProduct.Name} - {updatedProduct.Price:C} - {updatedProduct.AvailableQuantity} in stock");
                }

                var productToDelete = updatedProducts.FirstOrDefault();
                if (productToDelete != null)
                {
                    Console.WriteLine($"Deleting product with Id {productToDelete.Id}");
                    productService.DeleteProduct(productToDelete.Id);
                }

                var remainingProducts = productService.GetProducts();
                Console.WriteLine("List of remaining products:");
                foreach (var remainingProduct in remainingProducts)
                {
                    Console.WriteLine($"{remainingProduct.Id} - {remainingProduct.Name} - {remainingProduct.Price:C} - {remainingProduct.AvailableQuantity} in stock");
                }
            }
        }
    }
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public byte[] Timestamp { get; set; }
    }
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=TimestampDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product updatedProduct)
        {
            var existingProduct = _context.Products.Find(updatedProduct.Id);

            if (existingProduct != null)
            {
                _context.Entry(existingProduct).OriginalValues["Timestamp"] = updatedProduct.Timestamp;
                existingProduct.Name = updatedProduct.Name;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.Price = updatedProduct.Price;
                existingProduct.AvailableQuantity = updatedProduct.AvailableQuantity;

                _context.SaveChanges();
            }
        }

        public void DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
    }
}

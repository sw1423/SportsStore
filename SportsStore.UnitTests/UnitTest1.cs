using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1,Name = "P1"},
                new Product { ProductID = 2,Name = "P2"},
                new Product {ProductID = 3,Name = "P3" },
                new Product { ProductID = 4,Name = "P4"},
                new Product {ProductID = 5,Name = "P5"}
            });
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }
        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            HtmlHelper myHelper = null;
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };
            Func<int, string> pageUrlDelegate = i => "Page" + i;
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);
            //.断言
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
+ @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
+ @"<a class=""btn btn-default"" href=""Page3"">3</a>",
result.ToString());
            ;
        }
        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //准备.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1,Name = "P1"},
                new Product {ProductID = 2,Name = "P2"},
                new Product {ProductID = 3,Name = "P3" },
                new Product {ProductID = 4,Name = "P4" },
                new Product {ProductID = 5,Name = "P5"}
             });
            //准备
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;
            //动作
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;
            //断言
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }
        [TestMethod]
        public void Can_Filter_Products()
        {
            //准备一创建模仿存储库
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1,Name = "P1",Category = "Cat1"},
                new Product {ProductID = 2,Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3,Name = "P3",Category = "Cat1"} ,
                new Product {ProductID = 4,Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5,Name = "P5", Category = "Cat3"}
            });
            //准备一创建控制器，并使页面大小为3个物品
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;
            //动作
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();
            //断言
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }
        [TestMethod]
        public void Can_Create_Categories()
        {
            //准备- -创建模仿存储库
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1",Category = "Apples"},
                new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"},
             });
            //准备一 创建控制器
            NavController target = new NavController(mock.Object);
            //动作一获取分类集合
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();
            //断言
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }
        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //准备一创建模仿存储库
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name ="P1",Category = "Apples"},
                new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
            });
            //准备一创建控制器
            NavController target = new NavController(mock.Object);
            //准备一定义已选分类
            string categoryToSelect = "Apples";
            //动作
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;
            //断言
            Assert.AreEqual(categoryToSelect, result);
        }
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //准备-创建一些测试产品
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            //准备一创建一个新的购物车
            Cart target = new Cart();
            //动作
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();
            //断言
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }
        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //准备一创建一些测试产品
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product
            {
                ProductID = 2,
                Name = "P2"
            };
            //准备一创建新购物车
            Cart target = new Cart();
            //动作
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();
            //断言
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }
        [TestMethod]
        public void Can_Remove_Line()
        {
            //准备一创建一些测试产品
            Product p1 = new Product
            {
                ProductID = 1,
                Name = "P1"
            };
            Product p2 = new Product
            {
                ProductID = 2,
                Name = "P2"
            };
            Product p3 = new Product
            {
                ProductID = 3,
                Name = "P3"
            };
            //准备一创建一个新的购物车
            Cart target = new Cart();
            //准备一对购物车添加一-些产品
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);
            //动作
            target.Removeline(p2);
            //断言
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }
        public void Calculate_Cart_Total()
        {
            //准备一-创建一些测试产品
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };
            //准备一创建一个新的购物车
            Cart target = new Cart();
            //动作
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();
            //断言
            Assert.AreEqual(result, 450M);
        }
        [TestMethod]
        public void Can_Clear_Contents()
        {
            //准备一创建一些测试产品
            Product p1 = new Product
            {
                ProductID = 1,
                Name = "P1",
                Price = 100M
            };
            Product p2 = new Product
            {
                ProductID = 2,
                Name = "P2",
                Price = 50M
            };
            //准备一创建一个新的购物车
            Cart target = new Cart();
            //准备一添加一些物品
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            //动作一重置购物车
            target.Clear();
            //断言
            Assert.AreEqual(target.Lines.Count(), 0);
        }
    }
}

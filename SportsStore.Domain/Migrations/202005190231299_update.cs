namespace SportsStore.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        LastModifierId = c.Int(),
                        LastModifyTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Account = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 100),
                        Email = c.String(maxLength: 200),
                        Mobile = c.String(maxLength: 30),
                        CompanyId = c.Int(nullable: false),
                        CompanyName = c.String(maxLength: 500),
                        State = c.Int(nullable: false),
                        UserType = c.Int(nullable: false),
                        LastLoginTime = c.DateTime(),
                        CreateTime = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        LastModifierId = c.Int(),
                        LastModifyTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            //CreateTable(
            //    "dbo.Products",
            //    c => new
            //        {
            //            ProductID = c.Int(nullable: false, identity: true),
            //            Name = c.String(nullable: false),
            //            Description = c.String(nullable: false),
            //            Price = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            Category = c.String(nullable: false),
            //            ImageData = c.Binary(),
            //            ImageMImeType = c.String(),
            //        })
            //    .PrimaryKey(t => t.ProductID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.User", "CompanyId", "dbo.Company");
            DropIndex("dbo.User", new[] { "CompanyId" });
            //DropTable("dbo.Products");
            DropTable("dbo.User");
            DropTable("dbo.Company");
        }
    }
}

namespace Web_API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageDataMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ImageDataModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        ImageData = c.Binary(),
                        Tag = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ImageDataModels");
        }
    }
}

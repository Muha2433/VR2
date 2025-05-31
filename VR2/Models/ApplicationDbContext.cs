using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VR2.Models;

namespace VR2.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define DbSet properties for each model
        public DbSet<ModelAgent> Agents { get; set; }
        public DbSet<ModelAgentDocument> AgentDocuments { get; set; }
        public DbSet<ModelApartment> Apartments { get; set; }
        public DbSet<ModelAuctionBid> AuctionBids { get; set; }
        public DbSet<ModelAuctionBidParticipant> AuctionBidParticipants { get; set; }
        public DbSet<ModelCar> Cars { get; set; }
        public DbSet<ModelCarOwnership> CarOwnerships { get; set; }
        public DbSet<ModelCustomer> Customers { get; set; }
        public DbSet<ModelExternalOwner> ExternalOwners { get; set; }
        public DbSet<ModelFeatures> Features { get; set; }
        public DbSet<ModelFile> Files { get; set; }
        
        public DbSet<ModelHouse> Houses { get; set; }
        public DbSet<ModelLocation> Locations { get; set; }
        public DbSet<ModelProperties> Properties { get; set; }
        public DbSet<ModelPropertyOwnership> PropertyOwnerships { get; set; }
        public DbSet<ModelRequestForSale> RequestForSales { get; set; }
        public DbSet<ModelSafetyFeatures> SafetyFeatures { get; set; }
        public DbSet<ModelShare> Shares { get; set; }

        public DbSet<ModelVilla> Villas { get; set; }

        public DbSet<ModelPurchaseCustomer> PurchaseCustomers { get; set; }

        public DbSet<ModelFavorit> Favorits { get; set; }

        public DbSet<ModelClick> Clicks { get; set; }
        public DbSet<ModelContactInfo> ContactInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Configure Identity tables

            modelBuilder.Entity<ModelAgent>().ToTable("Agents");
            modelBuilder.Entity<ModelCustomer>().ToTable("Customers");


            modelBuilder.Entity<ModelClick>().HasOne(click=>click.Customer)
                .WithMany(customer=>customer.lstClick).HasForeignKey(click=>click.CustomerID).
                OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<ModelClick>().HasOne(click => click.RequestForSale)
               .WithMany(req=>req.lstClick).HasForeignKey(click => click.RequestID).
               OnDelete(DeleteBehavior.NoAction);


            //////
            modelBuilder.Entity<ModelFavorit>().HasOne(fav => fav.Customer)
               .WithMany(customer => customer.lstFavorit).HasForeignKey(fav => fav.CustomerID).
               OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ModelFavorit>().HasOne(fav => fav.RequestForSale)
              .WithMany(req => req.lstFavorit).HasForeignKey(fav => fav.RequestID).
              OnDelete(DeleteBehavior.NoAction);

            // modelBuilder.Entity<ModelProperties>()
            //.HasDiscriminator<string>("PropertyType")
            //.HasValue<ModelVilla>("Villas")
            //.HasValue<ModelApartment>("Apartment")
            //.HasValue<ModelHouse>("House");

            modelBuilder.Entity<ModelVilla>().ToTable("Villas");
            modelBuilder.Entity<ModelHouse>().ToTable("House");
            modelBuilder.Entity<ModelApartment>().ToTable("Apartment");



            modelBuilder.Entity<ModelContactInfo>()
                .HasOne(contactinfo => contactinfo.ExternalOwner)
                .WithOne(externalowner => externalowner.ContactInfo)
                .HasForeignKey<ModelExternalOwner>(externalowner => externalowner.ContactInfoID)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<ModelShare>()
                .HasOne(share => share.Property)
                .WithMany(p=>p.lstShare).HasForeignKey(share=>share.PropertyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ModelShare>()
          .HasOne(share => share.Request)
          .WithMany(r => r.lstShare).HasForeignKey(share => share.RequestID)
          .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ModelProperties>()
                .HasOne(p=>p.Location)
                .WithMany(location => location.lstProperties).HasForeignKey(p=>p.LocationID).OnDelete(DeleteBehavior.NoAction);




            modelBuilder.Entity<ModelPurchaseCustomer>()
                .HasOne(pc=>pc.Purchase).WithMany(p=>p.lstPurchaseCustomer).HasForeignKey(pc=>pc.PurchaseId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ModelPurchaseCustomer>()
              .HasOne(pc => pc.Buyer).WithMany(c => c.lstPurchaseCustomer).HasForeignKey(pc => pc.BuyerID).OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<ModelCarRequestSale>()
            //   .HasMany(a => a.lstCars)
            //   .WithOne(d => d.RequstSaleCar)
            //   .HasForeignKey(d => d.RequsetSaleID)
            //   .OnDelete(DeleteBehavior.NoAction);


            //modelBuilder.Entity<ModelPropertyRequestSale>()
            //   .HasOne(a => a.Property)
            //   .WithOne(d => d.RequstSaleProperty)
            //   .HasForeignKey<ModelProperties>(d => d.RequsetSaleID)
            //   .OnDelete(DeleteBehavior.NoAction);

            //// Configure relationships, keys, and constraints

            //// Configure the relationship between ModelAgent and ModelAgentDocument
            //modelBuilder.Entity<ModelAgent>()
            //    .HasMany(a => a.lstAgentDocument)
            //    .WithOne(d => d.Agent)
            //    .HasForeignKey(d => d.AgentID)
            //    .OnDelete(DeleteBehavior.NoAction); // Optional: Configure delete behavior

            //// Configure the relationship between ModelAgentDocument and ModelFile
            //modelBuilder.Entity<ModelAgentDocument>()
            //    .HasMany(d => d.lstFile)
            //    .WithOne()
            //    .HasForeignKey(f => f.AgentDocumentID)
            //    .OnDelete(DeleteBehavior.NoAction); // Optional: Configure delete behavior
            //// ModelApartment
            //modelBuilder.Entity<ModelApartment>()
            //    .HasBaseType<ModelProperties>();

            //// ModelAuctionBid
            //////modelBuilder.Entity<ModelAuctionBid>()
            //////    .HasOne(b => b.RequestForSale)
            //////    .WithMany(r => r.lstAuctionBid)
            //////    .HasForeignKey(b => b.RequestID).OnDelete(DeleteBehavior.NoAction); 

            //modelBuilder.Entity<ModelAuctionBid>()
            //    .HasMany(b => b.lstParticipants)
            //    .WithOne(p => p.Bid)
            //    .HasForeignKey(p => p.BidID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelAuctionBidParticipant
            //modelBuilder.Entity<ModelAuctionBidParticipant>()
            //    .HasOne(p => p.Buyer)
            //    .WithMany()
            //    .HasForeignKey(p => p.BuyerID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelCar
            //modelBuilder.Entity<ModelCar>()
            //    .HasMany(c => c.lstCarOwnership)
            //    .WithOne(o => o.Car)
            //    .HasForeignKey(o => o.CarID).OnDelete(DeleteBehavior.NoAction); ;

            //modelBuilder.Entity<ModelCar>()
            //    .HasMany(c => c.lstCarImages)
            //    .WithOne()
            //    .HasForeignKey(f => f.CarID).OnDelete(DeleteBehavior.NoAction); ;

            //modelBuilder.Entity<ModelCar>()
            //    .HasOne(c => c.Features)
            //    .WithOne(f => f.Car)
            //    .HasForeignKey<ModelFeatures>(f => f.CarID).OnDelete(DeleteBehavior.NoAction); ;

            //modelBuilder.Entity<ModelCar>()
            //    .HasOne(c => c.SafetyFeatures)
            //    .WithOne(s => s.Car)
            //    .HasForeignKey<ModelSafetyFeatures>(s => s.CarID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelCarOwnership
            //modelBuilder.Entity<ModelCarOwnership>()
            //    .HasOne(o => o.Customer)
            //    .WithMany(c => c.lstCarOwnership)
            //    .HasForeignKey(o => o.CustomerID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelCarRequestSale
            //modelBuilder.Entity<ModelCarRequestSale>()
            //    .HasBaseType<ModelRequestForSale>();

            //// ModelCustomer
            //modelBuilder.Entity<ModelCustomer>()
            //    .HasMany(c => c.lstPropertyOwnership)
            //    .WithOne(o => o.Seller)
            //    .HasForeignKey(o => o.CustomerID).OnDelete(DeleteBehavior.NoAction); ;

            //modelBuilder.Entity<ModelCustomer>()
            //    .HasMany(c => c.lstShareAsBuyer)
            //    .WithOne(s => s.Buyer)
            //    .HasForeignKey(s => s.BuyerID).OnDelete(DeleteBehavior.NoAction); ;

            //modelBuilder.Entity<ModelCustomer>()
            //    .HasMany(c => c.lstShareAsSeller)
            //    .WithOne(s => s.Seller)
            //    .HasForeignKey(s => s.SellerID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelExternalOwner
            //modelBuilder.Entity<ModelExternalOwner>()
            //    .HasMany(e => e.lstShareAsSeller)
            //    .WithOne(s => s.ExternalOwner)
            //    .HasForeignKey(s => s.ExternalOwnerID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelFile
            //modelBuilder.Entity<ModelFile>()
            //    .HasOne(f => f.AppUser)
            //    .WithMany(u => u.lstPersonalImage_IdentityImage)
            //    .HasForeignKey(f => f.AppUserID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelGroupResale
            //modelBuilder.Entity<ModelGroupResale>()
            //    .HasMany(g => g.lstGroupResaleMembers)
            //    .WithOne(m => m.GroupResale)
            //    .HasForeignKey(m => m.GroupResaleID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelGroupResaleMembers
            //modelBuilder.Entity<ModelGroupResaleMembers>()
            //    .HasOne(m => m.Share)
            //    .WithMany(s => s.lstGroupResaleMembers)
            //    .HasForeignKey(m => m.ShareID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelLocation
            //modelBuilder.Entity<ModelLocation>()
            //    .HasMany(l => l.lstProperties)
            //    .WithOne(p => p.Location)
            //    .HasForeignKey(p => p.LocationID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelProperties
            //modelBuilder.Entity<ModelProperties>()
            //    .HasMany(p => p.lstPropertyOwnership)
            //    .WithOne(o => o.Property)
            //    .HasForeignKey(o => o.PropertyID).OnDelete(DeleteBehavior.NoAction); ;

            //modelBuilder.Entity<ModelProperties>()
            //    .HasMany(p => p.lstPropertiesImage)
            //    .WithOne()
            //    .HasForeignKey(f => f.PropertyID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelPropertyOwnership
            //modelBuilder.Entity<ModelPropertyOwnership>()
            //    .HasOne(o => o.Seller)
            //    .WithMany(c => c.lstPropertyOwnership)
            //    .HasForeignKey(o => o.CustomerID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelPropertyRequestSale
            //modelBuilder.Entity<ModelPropertyRequestSale>()
            //    .HasBaseType<ModelRequestForSale>();

            //// ModelRequestForSale
            //modelBuilder.Entity<ModelRequestForSale>()
            //    .HasMany(r => r.lstShare)
            //    .WithOne(s => s.Request)
            //    .HasForeignKey(s => s.RequestID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelShare
            //modelBuilder.Entity<ModelShare>()
            //    .HasMany(s => s.lstShareResale)
            //    .WithOne(r => r.OriginalShare)
            //    .HasForeignKey(r => r.OriginalShareID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelShareResale
            //////////modelBuilder.Entity<ModelShareResale>()
            //////////    .HasOne(r => r.ShareResalBuyer)
            //////////    .WithMany(b => b.lstShareResale)
            //////////    .HasForeignKey(r => r.ShareResalBuyerID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelShareResaleBuyer
            //modelBuilder.Entity<ModelShareResaleBuyer>()
            //    .HasOne(b => b.Buyer)
            //    .WithMany()
            //    .HasForeignKey(b => b.BuyerID).OnDelete(DeleteBehavior.NoAction); ;

            //// ModelVilla
            //modelBuilder.Entity<ModelVilla>()
            //    .HasBaseType<ModelProperties>();
        }
    }
}
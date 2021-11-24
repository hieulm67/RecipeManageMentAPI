using Microsoft.EntityFrameworkCore;
using RecipeManagementBE.Constant;
using RecipeManagementBE.Entity;

#nullable disable

namespace RecipeManagementBE.Repository {
    public class StaffMateContext : DbContext {
        public StaffMateContext() {
        }

        public StaffMateContext(DbContextOptions<StaffMateContext> options)
            : base(options) {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Dish> Dishes { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<ProcessingStep> ProcessingSteps { get; set; }
        public virtual DbSet<Qa> Qas { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeDetail> RecipeDetails { get; set; }
        public virtual DbSet<RecipeTool> RecipeTools { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Tool> Tools { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                // optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=StaffMate;User ID=sa;Password=123456");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity => {
                entity.HasKey(e => e.UID)
                    .HasName("PK__Account__536C85E5FC9F8FDE");

                entity.Property(e => e.UID).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Account__RoleId__1367E606");
            });

            modelBuilder.Entity<Admin>(entity => {
                entity.Property(e => e.UID).IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.Admin)
                    .HasForeignKey<Admin>(d => d.UID)
                    .HasConstraintName("FK__Admin__AccountUs__1BFD2C07");
            });

            modelBuilder.Entity<Brand>(entity => {
                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.HomePage).IsUnicode(false);

                entity.Property(e => e.Logo).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                // entity.HasIndex(a => a.Name).IsUnique(); // Rang buoc bang code
            });

            modelBuilder.Entity<Category>(entity => {
                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK__Category__BrandI__1ED998B2");
                
                // entity.HasIndex(a => new { a.Name, a.BrandId }).IsUnique(); // Rang buoc bang code
            });

            modelBuilder.Entity<Dish>(entity => {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Dishes)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Dish__CategoryId__21B6055D");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.Dishes)
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Dish__ManagerId__22AA2996");

                // entity.HasIndex(a => new { a.Name, a.CategoryId }).IsUnique(); // Rang buoc bang code
            });

            modelBuilder.Entity<Employee>(entity => {
                entity.Property(e => e.UID).IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.Employee)
                    .HasForeignKey<Employee>(d => d.UID)
                    .HasConstraintName("FK__Employee__Accoun__182C9B23");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Employee__BrandI__1920BF5C");
            });

            modelBuilder.Entity<Log>(entity => { entity.Property(e => e.Type).IsUnicode(false); });

            modelBuilder.Entity<Notification>(entity => {
                entity.Property(e => e.To).IsUnicode(false);
                entity.Property(e => e.Type).IsUnicode(false);

                entity.HasOne(d => d.ToAccount)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.To)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Notification__To__35BCFE0A");
            });

            modelBuilder.Entity<ProcessingStep>(entity => {
                entity.Property(e => e.ImageDescription).IsUnicode(false);

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.ProcessingSteps)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK__Processin__Recip__286302EC");

                // entity.HasIndex(e => new { e.StepNumber, e.RecipeId }).IsUnique(); // Rang buoc bang code
            });

            modelBuilder.Entity<Qa>(entity => {
                entity.Property(e => e.UID).IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Qas)
                    .HasForeignKey(d => d.UID)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__QA__AccountUsern__31EC6D26");

                entity.HasOne(d => d.QaParent)
                    .WithOne(p => p.QaChild)
                    .HasForeignKey<Qa>(d => d.QaParentId)
                    .HasConstraintName("FK__QA__QAId__30F848ED");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Qas)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK__QA__RecipeId__32E0915F");
            });

            modelBuilder.Entity<Recipe>(entity => {
                entity.Property(e => e.ImageDescription).IsUnicode(false);

                entity.HasOne(d => d.Dish)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.DishId)
                    .HasConstraintName("FK__Recipe__DishId__25869641");
            });
            
            // Rang buoc bang code
            // modelBuilder.Entity<Ingredient>(entity => {
            //     entity.HasIndex(e => e.Name).IsUnique();
            // });

            modelBuilder.Entity<RecipeDetail>(entity => {
                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.RecipeDetails)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("FK__RecipeDet__Ingre__2E1BDC42");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeDetails)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK__RecipeDet__Recip__2D27B809");

                // Rang buoc bang code
                // entity.HasIndex(recipeDetail => new {recipeDetail.IngredientId, recipeDetail.RecipeId})
                //     .IsUnique();
            });

            // Rang buoc bang code
            // modelBuilder.Entity<Tool>(entity => {
            //     entity.HasIndex(e => e.Name).IsUnique();
            // });

            modelBuilder.Entity<RecipeTool>(entity => {
                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeTools)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK__RecipeToo__Recip__3C69FB99");

                entity.HasOne(d => d.Tool)
                    .WithMany(p => p.RecipeTools)
                    .HasForeignKey(d => d.ToolId)
                    .HasConstraintName("FK__RecipeToo__ToolI__3D5E1FD2");

                // Rang buoc bang code
                // entity.HasIndex(recipeTool => new {recipeTool.ToolId, recipeTool.RecipeId})
                //     .IsUnique();
            });

            modelBuilder.Entity<Role>(entity => { entity.Property(e => e.Code).IsUnicode(false); });

            modelBuilder.Entity<Role>().HasData(
                new Role {
                    Id = 1,
                    Code = RoleConstants.ADMIN_CODE,
                    Name = "Admin"
                },
                new Role {
                    Id = 2,
                    Code = RoleConstants.MANAGER_CODE,
                    Name = "Manager"
                },
                new Role {
                    Id = 3,
                    Code = RoleConstants.STAFF_CODE,
                    Name = "Staff"
                });

            modelBuilder.Entity<Account>().HasData(
                new Account {
                    UID = "fxOOYx2XASRKC2A77aNG2wKJhAy2",
                    Email = "staffmatesum2021@gmail.com",
                    Password = "$2a$11$gQLzmk8hWrWEYNFppMaWVugdn6rWlluDs/PtvujwnxTIf.rMxEk4O", // 123456
                    RoleId = RoleConstants.ADMIN_ID,
                    FullName = "Admin System"
                }
            );
            
            modelBuilder.Entity<Admin>().HasData(
                new Admin {
                    Id = 1,
                    UID = "fxOOYx2XASRKC2A77aNG2wKJhAy2"
                }
            );
        }
    }
}
using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<LocalUser> LocalUsers { get; set; }
    
    public DbSet<Villa> Villas { get; set; }
    
    public DbSet<VillaNumber> VillaNumbers { get; set; }
    
}
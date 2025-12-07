using Grosu_Andrada_Lab4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.Collections.Generic;

namespace Grosu_Andrada_Lab4.Data
{
    public class Grosu_Andrada_Lab4Context : DbContext
    {
        public Grosu_Andrada_Lab4Context(DbContextOptions<Grosu_Andrada_Lab4Context> options)
            : base(options)
        {
        }

        public DbSet<PredictionHistory> PredictionHistory { get; set; } = default!;
    }
}

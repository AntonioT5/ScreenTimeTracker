using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IPredictionService
    {
        Task GeneratePredictionAsync(Guid userId);
    }
}
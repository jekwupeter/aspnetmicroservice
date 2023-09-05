using AutoMapper;
using Discount.Grpc;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly ILogger<DiscountService> _log;
        private readonly IDiscount _repo;
        private readonly IMapper _map;
        public DiscountService(ILogger<DiscountService> log, IMapper map, IDiscount repo)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repo.GetDiscount(request.ProductName);

            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with product name {request.ProductName} not found"));
            }

            _log.LogInformation("Discount is retrieved for ProductName:{name}", request.ProductName);
            var couponModel = _map.Map<CouponModel>(coupon);
            return couponModel;
        }
        
        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _map.Map<Coupon>(request.Coupon);

            await _repo.CreateDiscount(coupon);

            var couponModel = _map.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _map.Map<Coupon>(request.Coupon);

            await _repo.UpdateDiscount(coupon);

            var couponModel = _map.Map<CouponModel>(coupon);

            return couponModel;
        } 
        
        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await _repo.DeleteDiscount(request.ProductName);

            return new DeleteDiscountResponse { Success = deleted} ;
        }
    }
}
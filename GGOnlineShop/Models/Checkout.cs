using System.ComponentModel.DataAnnotations;

namespace GGOnlineShop.Models
{
    public class CheckoutViewModel
    {
        // 支付方式枚举（对应前端Mastercard、Visa、PayPal、货到付款）
        public enum PaymentMethod
        {
            Mastercard,
            Visa,
            PayPal,
            CashOnDelivery
        }

        // 支付方式选择
        public PaymentMethod SelectedPaymentMethod { get; set; }

        // 信用卡信息（仅当选择信用卡支付时需要）
        [Display(Name = "Card number")]
        public string CardNumber { get; set; }

        [Display(Name = "Cardholder")]
        public string CardholderName { get; set; }

        [Display(Name = "Expiry month")]
        public int ExpiryMonth { get; set; }

        [Display(Name = "Expiry year")]
        public int ExpiryYear { get; set; }

        [Display(Name = "CVC")]
        public string Cvc { get; set; }

        // 订单总额（从购物车传递）
        public decimal TotalAmount { get; set; }

        // 保存支付信息（可选）
        public bool SavePaymentDetails { get; set; }
    }
}
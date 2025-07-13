using System;

namespace CYS.Models.Mobil
{
    public class ApiResponse<T>
    {
        // İşlem başarılı mı?
        public bool Success { get; set; }

        // Kullanıcıya gösterilecek mesaj (başarı ya da hata durumu)
        public string Message { get; set; }

        // İşlem sonucu döndürülen veriler
        public T Data { get; set; }

        // Hata kodu (hata durumunda)
        public int? ErrorCode { get; set; }

        // API dönüş zaman damgası
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Standart başarı cevabı oluşturucu
        public static ApiResponse<T> SuccessResponse(T data, string message = "İşlem başarılı")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                ErrorCode = null
            };
        }

        // Standart hata cevabı oluşturucu
        public static ApiResponse<T> ErrorResponse(string message, int? errorCode = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                ErrorCode = errorCode
            };
        }
    }
}

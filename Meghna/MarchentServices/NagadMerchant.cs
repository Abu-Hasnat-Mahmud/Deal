using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.MarchentServices
{
    public  class NagadMerchant
    {
        // Sandbox
        public static string S_MerchantId = "683002007104225";
        public static string S_NGPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjBH1pFNSSRKPuMcNxmU5jZ1x8K9LPFM4XSu11m7uCfLUSE4SEjL30w3ockFvwAcuJffCUwtSpbjr34cSTD7EFG1Jqk9Gg0fQCKvPaU54jjMJoP2toR9fGmQV7y9fz31UVxSk97AqWZZLJBT2lmv76AgpVV0k0xtb/0VIv8pd/j6TIz9SFfsTQOugHkhyRzzhvZisiKzOAAWNX8RMpG+iqQi4p9W9VrmmiCfFDmLFnMrwhncnMsvlXB8QSJCq2irrx3HG0SJJCbS5+atz+E1iqO8QaPJ05snxv82Mf4NlZ4gZK0Pq/VvJ20lSkR+0nk+s/v3BgIyle78wjZP1vWLU4wIDAQAB";
        public static string S_MSPrivateKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCJakyLqojWTDAVUdNJLvuXhROV+LXymqnukBrmiWwTYnJYm9r5cKHj1hYQRhU5eiy6NmFVJqJtwpxyyDSCWSoSmIQMoO2KjYyB5cDajRF45v1GmSeyiIn0hl55qM8ohJGjXQVPfXiqEB5c5REJ8Toy83gzGE3ApmLipoegnwMkewsTNDbe5xZdxN1qfKiRiCL720FtQfIwPDp9ZqbG2OQbdyZUB8I08irKJ0x/psM4SjXasglHBK5G1DX7BmwcB/PRbC0cHYy3pXDmLI8pZl1NehLzbav0Y4fP4MdnpQnfzZJdpaGVE0oI15lq+KZ0tbllNcS+/4MSwW+afvOw9bazAgMBAAECggEAIkenUsw3GKam9BqWh9I1p0Xmbeo+kYftznqai1pK4McVWW9//+wOJsU4edTR5KXK1KVOQKzDpnf/CU9SchYGPd9YScI3n/HR1HHZW2wHqM6O7na0hYA0UhDXLqhjDWuM3WEOOxdE67/bozbtujo4V4+PM8fjVaTsVDhQ60vfv9CnJJ7dLnhqcoovidOwZTHwG+pQtAwbX0ICgKSrc0elv8ZtfwlEvgIrtSiLAO1/CAf+uReUXyBCZhS4Xl7LroKZGiZ80/JE5mc67V/yImVKHBe0aZwgDHgtHh63/50/cAyuUfKyreAH0VLEwy54UCGramPQqYlIReMEbi6U4GC5AQKBgQDfDnHCH1rBvBWfkxPivl/yNKmENBkVikGWBwHNA3wVQ+xZ1Oqmjw3zuHY0xOH0GtK8l3Jy5dRL4DYlwB1qgd/Cxh0mmOv7/C3SviRk7W6FKqdpJLyaE/bqI9AmRCZBpX2PMje6Mm8QHp6+1QpPnN/SenOvoQg/WWYM1DNXUJsfMwKBgQCdtddE7A5IBvgZX2o9vTLZY/3KVuHgJm9dQNbfvtXw+IQfwssPqjrvoU6hPBWHbCZl6FCl2tRh/QfYR/N7H2PvRFfbbeWHw9+xwFP1pdgMug4cTAt4rkRJRLjEnZCNvSMVHrri+fAgpv296nOhwmY/qw5Smi9rMkRY6BoNCiEKgQKBgAaRnFQFLF0MNu7OHAXPaW/ukRdtmVeDDM9oQWtSMPNHXsx+crKY/+YvhnujWKwhphcbtqkfj5L0dWPDNpqOXJKV1wHt+vUexhKwus2mGF0flnKIPG2lLN5UU6rs0tuYDgyLhAyds5ub6zzfdUBG9Gh0ZrfDXETRUyoJjcGChC71AoGAfmSciL0SWQFU1qjUcXRvCzCK1h25WrYS7E6pppm/xia1ZOrtaLmKEEBbzvZjXqv7PhLoh3OQYJO0NM69QMCQi9JfAxnZKWx+m2tDHozyUIjQBDehve8UBRBRcCnDDwU015lQN9YNb23Fz+3VDB/LaF1D1kmBlUys3//r2OV0Q4ECgYBnpo6ZFmrHvV9IMIGjP7XIlVa1uiMCt41FVyINB9SJnamGGauW/pyENvEVh+ueuthSg37e/l0Xu0nm/XGqyKCqkAfBbL2Uj/j5FyDFrpF27PkANDo99CdqL5A4NQzZ69QRlCQ4wnNCq6GsYy2WEJyU2D+K8EBSQcwLsrI7QL7fvQ==";
                             
        public static string S_InitializeUri = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/initialize/"; // {0} = marchentid, {1} = orderId
        public static string S_CheckOutUri = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/complete/";  // {0} = paymentRefId
        public static string S_CallbackUri = $"Nagad/PaymentResult";
        public static string S_VerifyUri = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/verify/payment/"; // {0} = paymentRefId
                             
        public static string S_CancelInitializeUri = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/verify/payment/"; // {0} = paymentRefId
        public static string S_AccountNumber = "01711428036";


        ///////Live
        public static string MerchantId = "686130193825734";
        public static string NGPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAiCWvxDZZesS1g1lQfilVt8l3X5aMbXg5WOCYdG7q5C+Qevw0upm3tyYiKIwzXbqexnPNTHwRU7Ul7t8jP6nNVS/jLm35WFy6G9qRyXqMc1dHlwjpYwRNovLc12iTn1C5lCqIfiT+B/O/py1eIwNXgqQf39GDMJ3SesonowWioMJNXm3o80wscLMwjeezYGsyHcrnyYI2LnwfIMTSVN4T92Yy77SmE8xPydcdkgUaFxhK16qCGXMV3mF/VFx67LpZm8Sw3v135hxYX8wG1tCBKlL4psJF4+9vSy4W+8R5ieeqhrvRH+2MKLiKbDnewzKonFLbn2aKNrJefXYY7klaawIDAQAB";
        public static string MSPrivateKey = "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCqAHZic6MecP91nw4XDitSO0otRJxJzbi4BGIyghe6Yhn160OpQHnUtpAgHpnt3tJZEQdYOY5YO2nQ1ykAYfEPEbSg3Z6U+u+7FbsYEOsc6rzdJ7JqAxbwprapkh3ENb+5kiBwrBBQwPbpVgeMInqX5Mr2nSQmljRfjqWKZbK6FeRIfab57kRvGwoki9JzimGs5lKEpywoVQbAjL/QZcwS9Qp6yejyRwh4Plc7Jaxf8k9GRO5JyEyQvZ1KwfziNFGLOnfD0LIPH9b1ffarPmC1kRECYNDmsWTrkN2PdYuj4SaTigkoGKyjiLem/7YLVSvv8x92wTEdE81Xx5yFQNY7AgMBAAECggEAagjBzmSMEJewbv/XPQAkezTp7lRGGy6KkZSCXziPbjx0LtQgLrg9hTSdrrsHjbuWfeFGMHwt0dC1DoK1Wzy7q4eCn3e7yva9gnZqbPdYfn/XbyWsfb0RmOaTNi8iC9jujeOcAksAHi5Nk3qKWJjE2GrnQW0AOGUlo5iInksScEfiXrFs4hFOQmhOQYJazYLeBncIe4nvUwcaE7Lseik7//G+CBupUFRl8Lv3QvNpfU2FBob/kOC6biz5xhdaFaUKr/VI8BPMR/G9kzSgV/Dt2cZP8n2fkcpdPbGQ3ckDwRQP90RUPJ6CWTH79u++vyqATi3FC23Y2uepwFScpghOUQKBgQDw1BWpzr0Pw4xThpR0S51Subq6n6fvh5ChU/rn1ciN3Asht8oZmP05PW2WCnj7aRmDjf4s8ii7r9c0VbflImLnGphWYHky41IaFrzqXLvdsmSx0S5yENr82XP4fm+Q/B6aeVtt8Hh2MhPGsiFsenP3ShuYkEOumQLQ6PWHIxdXSQKBgQC0tiJpwSHimXE2QHkXWc92vUeLRyMUPXnOkDq9tnPyIlx10tCBVaH5VT6dYwjwHNyAURX8sc961zi9WLWYb2EWCaZaJQEunzsuab2gDXHsVxYKc/QWbgPay+8y3Z60N98y5PqI/9uwjHjStug1nqzD49xhD1es1Z2pKG6WmpBtYwKBgQC2FMPkRSjwmDqqlB/95YWnHEGwBDImieqx7xrO2fXuO6Y28gxdWixqcKVAQd7Cxu3BZ0P7m4NslEAzk3OcTGlWrebrt7kq3nAexX5D+6UWs2AqiSuClnfboFVsVbvodJZ22LZl4uBRDP+ixN88c0DmgSNoL/rcMVfNt7SbXc6x8QKBgG0fBkzB2MpSSbCu4fAdxU84ILmTrTUNoj3/jdj9EaqG4TosoMdYhERhGzxCjyUe6G97h0FdxaGx0ItVw+JWb1O1ZARPEBWtBTmTlHPPapmCRMbvGddpe15lgv0+IhVFH7xF1JthlLFZQaRL2pB2TnDDaVIax6CQJXH9/jlnAAttAoGBANL/RJS1HWT9EX8MdnA5aq/uJ1nHhmrCzWN9dk33voOcJqfg4e4YEf3GvydDMbAW7A67o19nGSH+8gAf3OvYePjgxpvJA8qVQrCKA2DK8DHd8shdUYvoW8Q4vZnTiEkrPIGvpUhtsay1xCBbLxcpTWQ0/IYWg9zyVn9AtQ9jb1ky";
        public static string InitializeUri = $"https://api.mynagad.com/api/dfs/check-out/initialize/"; ///Mandatory
        public static string CheckOutUri = $"https://api.mynagad.com/api/dfs/check-out/complete/";  // {0} = paymentRefId
        public static string VerifyUri = $"https://api.mynagad.com/api/dfs/verify/payment/"; // {0} = paymentRefId
        public static string CallbackUri = $"Nagad/PaymentResult";

        public static string CancelInitializeUri = $"https://api.mynagad.com/api/dfs/purchase/cancel?"; // {0} = paymentRefId
        public static string AccountNumber = "01613019382";





    }
}

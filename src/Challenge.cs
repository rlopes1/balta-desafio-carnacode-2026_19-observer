// DESAFIO: Sistema de Monitoramento de Ações na Bolsa
// PROBLEMA: Um sistema financeiro precisa notificar múltiplos investidores quando o preço
// de ações muda. O código atual faz polling constante ou tem dependências diretas entre
// as ações e os investidores, criando acoplamento forte e código difícil de manter

using System;
using System.Collections.Generic;
using System.Threading;

namespace DesignPatternChallenge
{
    // Contexto: Sistema de trading onde investidores querem ser notificados de mudanças
    // em tempo real sem ter que ficar consultando constantemente (polling)


    public interface IObserver
    {
        void Update(string symbol, decimal price, decimal changePercent);
    }
    
    public class Stock 
    {

        private List<IObserver> observers = new List<IObserver>();
        public string Symbol { get; set; }
        public decimal Price { get; private set; }
        public DateTime LastUpdate { get; private set; }


        public Stock(string symbol, decimal initialPrice)
        {
            Symbol = symbol;
            Price = initialPrice;
            LastUpdate = DateTime.Now;
        }

        public void RegisterObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void UnregisterObserver(IObserver observer)
        {
            observers.Remove(observer);

        }

        public void UpdatePrice(decimal newPrice)
        {
            if (Price != newPrice)
            {
                decimal oldPrice = Price;
                Price = newPrice;
                LastUpdate = DateTime.Now;
                
                decimal changePercent = ((newPrice - oldPrice) / oldPrice) * 100;
                
                Console.WriteLine($"\n[{Symbol}] Preço atualizado: R$ {oldPrice:N2} → R$ {newPrice:N2} ({changePercent:+0.00;-0.00}%)");

                // Problema: Precisa notificar cada observador manualmente
                // e conhecer o tipo específico de cada um
              foreach (var observer in observers)
              {
                observer.Update(Symbol, newPrice, changePercent);
              }
            }
        }

        // Problema: Não há forma de remover observadores dinamicamente
        // Problema: Não suporta múltiplos observadores do mesmo tipo
    }

    public class Investor : IObserver
    {
        public string Name { get; set; }
        public decimal AlertThreshold { get; set; }

        public Investor(string name, decimal alertThreshold)
        {
            Name = name;
            AlertThreshold = alertThreshold;
        }

        public void Update(string symbol, decimal price, decimal changePercent)
        {
            Console.WriteLine($"  → [Investidor {Name}] Notificado sobre {symbol}");
            
            if (Math.Abs(changePercent) >= AlertThreshold)
            {
                Console.WriteLine($"  → [Investidor {Name}] ⚠️ ALERTA! Mudança de {changePercent:+0.00;-0.00}% excedeu limite de {AlertThreshold}%");
            }
        }
    }

    public class MobileApp : IObserver  
    {
        public string UserId { get; set; }

        public MobileApp(string userId)
        {
            UserId = userId;
        }

        public void Update(string symbol, decimal price, decimal changePercent)
        {
            Console.WriteLine($"  → [App Mobile {UserId}] 📱 Push: {symbol} agora em R$ {price:N2} ({changePercent:+0.00;-0.00}%)");
        }
    }

    public class TradingBot : IObserver
    {
        public string BotName { get; set; }
        public decimal BuyThreshold { get; set; }
        public decimal SellThreshold { get; set; }

        public TradingBot(string botName, decimal buyThreshold, decimal sellThreshold)
        {
            BotName = botName;
            BuyThreshold = buyThreshold;
            SellThreshold = sellThreshold;
        }

        public void Update(string symbol, decimal price, decimal changePercent)
        {
            Console.WriteLine($"  → [Bot {BotName}] 🤖 Analisando {symbol}...");
            
            if (changePercent <= -BuyThreshold)
            {
                Console.WriteLine($"  → [Bot {BotName}] 💰 COMPRANDO {symbol} por R$ {price:N2}");
            }
            else if (changePercent >= SellThreshold)
            {
                Console.WriteLine($"  → [Bot {BotName}] 💸 VENDENDO {symbol} por R$ {price:N2}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Monitoramento de Ações ===");

            var petr4 = new Stock("PETR4", 35.50m);

            // Problema: Precisa registrar cada observador individualmente
            var investor1 = new Investor("João Silva", 3.0m);
            var investor2 = new Investor("Maria Santos", 5.0m);
            var mobileApp = new MobileApp("user123");
            var tradingBot = new TradingBot("AlgoTrader", 2.0m, 2.5m);

            petr4.RegisterObserver(investor1);
            petr4.RegisterObserver(investor2);
            petr4.RegisterObserver(mobileApp);
            petr4.RegisterObserver(tradingBot);

            // Simulando mudanças de preço
            Console.WriteLine("\n=== Movimentações do Mercado ===");
            
            petr4.UpdatePrice(36.20m); // +1.97%
            Thread.Sleep(500);
            
            petr4.UpdatePrice(37.50m); // +3.59%
            Thread.Sleep(500);
            
            petr4.UpdatePrice(35.00m); // -6.67%
            Thread.Sleep(500);

            

            // Perguntas para reflexão:
            // - Como desacoplar objeto observado dos observadores?
            // - Como notificar múltiplos objetos automaticamente?
            // - Como permitir subscrição/cancelamento dinâmico?
            // - Como criar dependência um-para-muitos desacoplada?
        }
    }
}

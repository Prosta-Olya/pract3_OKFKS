using CinemaTicketSystem;

using Xunit;

namespace CinemaTicketSystemTests
{
    public class TicketRequestClassTests
    {
        // 1. Проверка корректных вычислений

        [Fact]
        public void CalculatePriceTicketNoDiscount() // обычный билет без скидок
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 40;
            ticketRequest.IsStudent = false;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            decimal extected_result = 300; // базовая стоимость билета
            decimal actual_result = ticketPriceCalculator.CalculatePrice(ticketRequest);

            Assert.Equal(extected_result, actual_result);
        }

        [Fact]
        // баг
        public void CalculatePriceTicketForChild() // детский билет
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 7;
            ticketRequest.IsStudent = false;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            decimal extected_result = 300 - ((300 * 40) / 100); // Скидка 40% для детей 6-17 лет
            decimal actual_result = ticketPriceCalculator.CalculatePrice(ticketRequest);

            Assert.Equal(extected_result, actual_result);
        }

        [Fact]
        public void CalculatePriceTicketForStudent() // студенческая скидка
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 20;
            ticketRequest.IsStudent = true;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            decimal extected_result = 300 - ((300 * 20) / 100); // Скидка 20% для студентов 18-25 лет
            decimal actual_result = ticketPriceCalculator.CalculatePrice(ticketRequest);

            Assert.Equal(extected_result, actual_result);
        }

        [Fact]
        public void CalculatePriceTicketInWednesday() //скидка по средам
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 40;
            ticketRequest.IsStudent = true;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Wednesday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            decimal extected_result = 300 - ((300 * 30) / 100); // Скидка в среду 30%
            decimal actual_result = ticketPriceCalculator.CalculatePrice(ticketRequest);

            Assert.Equal(extected_result, actual_result);
        }

        [Fact]
        public void CalculatePriceTicketInMorning() //утренняя скидка
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 40;
            ticketRequest.IsStudent = true;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(8);

            decimal extected_result = 300 - ((300 * 15) / 100); // Утренняя скидка (до 12:00) 15%
            decimal actual_result = ticketPriceCalculator.CalculatePrice(ticketRequest);

            Assert.Equal(extected_result, actual_result);
        }

        [Fact]
        public void CalculatePriceTicketForVip() // VIP-билет
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 40;
            ticketRequest.IsStudent = false;
            ticketRequest.IsVip = true;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            decimal extected_result = 300 + ((300 * 100) / 100); // VIP-наценка +100% применяется к итоговой цене после применения скидки
            decimal actual_result = ticketPriceCalculator.CalculatePrice(ticketRequest);

            Assert.Equal(extected_result, actual_result);
        }

        [Fact]
        // баг
        public void CalculatePriceTicketTwoDiscounts() // правило применения максимальной скидки
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 70; // Скидка 50% для пенсионеров 65+
            ticketRequest.IsStudent = false;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(8); // Утренняя скидка (до 12:00) 15%

            decimal extected_result = 300 - ((300 * 50) / 100); // Применяется только максимальная скидка
            decimal actual_result = ticketPriceCalculator.CalculatePrice(ticketRequest);

            Assert.Equal(extected_result, actual_result);
        }

        // 2. Проверка граничных значений

        [Fact]
        public void AgeMinAcceptable_NotShouldReturnException() // минимально допустимый возраст
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 0;
            ticketRequest.IsStudent = false;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            Assert.NotNull(() => ticketPriceCalculator.CalculatePrice(ticketRequest));
        }

        [Fact]
        public void AgeMaxAcceptable_NotShouldReturnException() // максимально допустимый возраст
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 120;
            ticketRequest.IsStudent = false;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            Assert.NotNull(() => ticketPriceCalculator.CalculatePrice(ticketRequest));
        }

        [Fact]
        public void AgeHigherAcceptable_CorrectDiscount() //возраст выше границы скидки
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 26;
            ticketRequest.IsStudent = false;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            decimal extected_result = 300; // Билет без скидки
            decimal actual_result = ticketPriceCalculator.CalculatePrice(ticketRequest);

            Assert.Equal(extected_result, actual_result);
        }

        [Fact]
        // баг
        public void AgeOnAcceptable_CorrectDiscount() //возраст на границе скидки
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 25;
            ticketRequest.IsStudent = true;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            decimal extected_result = 300 - ((300 * 20) / 100); // Скидка 20% для студентов 18-25 лет
            decimal actual_result = ticketPriceCalculator.CalculatePrice(ticketRequest);

            Assert.Equal(extected_result, actual_result);
        }

        // 3. Проверка исключений

        [Fact]
        public void AgeLessThanBorder_ShouldReturnException() // возраст меньше границы (меньше 0)
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = -1;
            ticketRequest.IsStudent = false;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            Assert.Throws<ArgumentOutOfRangeException>(() => ticketPriceCalculator.CalculatePrice(ticketRequest));
        }

        [Fact]
        public void AgeMoreThanBorder_ShouldReturnException() // возраст больше границы (больше 120)
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest.Age = 121;
            ticketRequest.IsStudent = false;
            ticketRequest.IsVip = false;
            ticketRequest.Day = DayOfWeek.Monday;
            ticketRequest.SessionTime = TimeSpan.FromHours(13);

            Assert.Throws<ArgumentOutOfRangeException>(() => ticketPriceCalculator.CalculatePrice(ticketRequest));
        }

        [Fact]
        public void EmptyRequest_ShouldReturnException() // пустой запрос
        {
            var ticketPriceCalculator = new TicketPriceCalculator();

            var ticketRequest = new TicketRequest();
            ticketRequest = null;

            Assert.Throws<ArgumentNullException>(() => ticketPriceCalculator.CalculatePrice(ticketRequest));
        }
    }
}

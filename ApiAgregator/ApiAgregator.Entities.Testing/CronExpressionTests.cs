using Xunit;

namespace ApiAgregator.Entities.Testing
{
    public class CronExpressionTests
    {
        [Fact]
        public void Test1()
        {
            var exp = new CronExpression("  *   *    *   *   * ");

            DateTime time = new DateTime();

            Assert.True(exp.Match(time));
            Assert.True(exp.Match(time.AddMinutes(100000)));
            Assert.True(exp.Match(time.AddMinutes(243556)));
            Assert.True(exp.Match(time.AddMinutes(655345)));
            Assert.True(exp.Match(time.AddMinutes(766554)));
            Assert.True(exp.Match(time.AddMinutes(876346)));
        }

        [Fact]
        public void Test2()
        {
            var exp = new CronExpression("  0   0    1   *   * ");

            DateTime time = new DateTime();

            Assert.True(exp.Match(time));

            Assert.True(exp.Match(time.AddMonths(60)));

            Assert.False(exp.Match(time.AddMinutes(1)));

            Assert.False(exp.Match(time.AddDays(10)));
        }

        [Fact]
        public void Test3()
        {
            var exp = new CronExpression("  *   *    *   JUL-AUG,OCT   *  ");

            DateTime time = new DateTime();

            Assert.False(exp.Match(time.AddMonths(5)));
            Assert.True(exp.Match(time.AddMonths(6)));
            Assert.True(exp.Match(time.AddMonths(7)));
            Assert.False(exp.Match(time.AddMonths(8)));
            Assert.True(exp.Match(time.AddMonths(9)));
        }

        [Fact]
        public void Test4()
        {
            var exp = new CronExpression("  */2   */2    *   *   *  ");

            DateTime time = new DateTime();

            Assert.True(exp.Match(time));
            Assert.False(exp.Match(time.AddMinutes(1)));
            Assert.False(exp.Match(time.AddMinutes(1).AddHours(1)));
            Assert.False(exp.Match(time.AddMinutes(1).AddHours(2)));
            Assert.True(exp.Match(time.AddMinutes(2)));
            Assert.False(exp.Match(time.AddMinutes(2).AddHours(1)));
            Assert.True(exp.Match(time.AddMinutes(2).AddHours(2)));
            Assert.False(exp.Match(time.AddMinutes(3)));
            Assert.False(exp.Match(time.AddMinutes(3).AddHours(1)));
            Assert.False(exp.Match(time.AddMinutes(3).AddHours(2)));
            Assert.True(exp.Match(time.AddMinutes(4)));
            Assert.False(exp.Match(time.AddMinutes(4).AddHours(1)));
            Assert.True(exp.Match(time.AddMinutes(4).AddHours(2)));
        }

        [Fact]
        public void Test5()
        {
            var exp = new CronExpression("  20-50/5   *    11,13,15-17/1   *   TUE-THU/2  ");
            DateTime time = new DateTime();

            time = time.AddYears(2021).AddMonths(6).AddDays(10);

            Assert.False(exp.Match(time.AddMinutes(15)));
            Assert.False(exp.Match(time.AddMinutes(20).AddDays(-1)));
            Assert.False(exp.Match(time.AddDays(7).AddMinutes(50)));
            Assert.False(exp.Match(time.AddDays(3).AddMinutes(10)));

            Assert.True(exp.Match(time.AddMinutes(20)));
            Assert.True(exp.Match(time.AddDays(1).AddMinutes(25)));
            Assert.True(exp.Match(time.AddDays(2).AddMinutes(30)));
            Assert.True(exp.Match(time.AddDays(3).AddMinutes(35)));
            Assert.True(exp.Match(time.AddDays(4).AddMinutes(40)));
            Assert.True(exp.Match(time.AddDays(5).AddMinutes(45)));
            Assert.True(exp.Match(time.AddDays(6).AddMinutes(50)));
        }
    }
}
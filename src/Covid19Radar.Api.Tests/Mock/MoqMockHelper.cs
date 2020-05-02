using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Tests.Mock
{
    internal static class MoqMockHelper
    {
        public static (TSut, Mock<TArg0>) Create<TSut, TArg0>()
            where TArg0 : class
        {
            var mock0 = new Mock<TArg0>();
            var sut = (TSut)Activator.CreateInstance(typeof(TSut), mock0.Object);
            return (sut, mock0);
        }

        public static (TSut, Mock<TArg0>, Mock<TArg1>) Create<TSut, TArg0, TArg1>()
            where TArg0 : class
            where TArg1 : class
        {
            var mock0 = new Mock<TArg0>();
            var mock1 = new Mock<TArg1>();
            var sut = (TSut)Activator.CreateInstance(typeof(TSut), mock0.Object, mock1.Object);
            return (sut, mock0, mock1);
        }

        public static (TSut, Mock<TArg0>, Mock<TArg1>, Mock<TArg2>) Create<TSut, TArg0, TArg1, TArg2>()
            where TArg0 : class
            where TArg1 : class
            where TArg2 : class
        {
            var mock0 = new Mock<TArg0>();
            var mock1 = new Mock<TArg1>();
            var mock2 = new Mock<TArg2>();
            var sut = (TSut)Activator.CreateInstance(typeof(TSut), mock0.Object, mock1.Object, mock2.Object);
            return (sut, mock0, mock1, mock2);
        }

        public static (TSut, Mock<TArg0>, Mock<TArg1>, Mock<TArg2>, Mock<TArg3>) Create<TSut, TArg0, TArg1, TArg2, TArg3>()
            where TArg0 : class
            where TArg1 : class
            where TArg2 : class
            where TArg3 : class
        {
            var mock0 = new Mock<TArg0>();
            var mock1 = new Mock<TArg1>();
            var mock2 = new Mock<TArg2>();
            var mock3 = new Mock<TArg3>();
            var sut = (TSut)Activator.CreateInstance(typeof(TSut), mock0.Object, mock1.Object, mock2.Object, mock3.Object);
            return (sut, mock0, mock1, mock2, mock3);
        }

        public static (TSut, Mock<TArg0>, Mock<TArg1>, Mock<TArg2>, Mock<TArg3>, Mock<TArg4>) Create<TSut, TArg0, TArg1, TArg2, TArg3, TArg4>()
            where TArg0 : class
            where TArg1 : class
            where TArg2 : class
            where TArg3 : class
            where TArg4 : class
        {
            var mock0 = new Mock<TArg0>();
            var mock1 = new Mock<TArg1>();
            var mock2 = new Mock<TArg2>();
            var mock3 = new Mock<TArg3>();
            var mock4 = new Mock<TArg4>();
            var sut = (TSut)Activator.CreateInstance(typeof(TSut), mock0.Object, mock1.Object, mock2.Object, mock3.Object, mock4.Object);
            return (sut, mock0, mock1, mock2, mock3, mock4);
        }
    }
}

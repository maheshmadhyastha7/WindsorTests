﻿using Castle.Windsor;
using NUnit.Framework;

namespace WindsorTests
{
    public abstract class AbstractWindsorContainerPerTest
    {
        private IWindsorContainer _windsorContainer;

        protected IWindsorContainer WindsorContainer
            => _windsorContainer ?? (_windsorContainer = CreateWindsorContainer());

        protected abstract IWindsorContainer CreateWindsorContainer();

        protected void ClearContainer()
        {
            _windsorContainer?.Dispose();
            _windsorContainer = null;
        }

        [SetUp]
        public void SetUp()
        {
            ClearContainer();
        }

        [TearDown]
        public void TearDown()
        {
            ClearContainer();
        }
    }
}
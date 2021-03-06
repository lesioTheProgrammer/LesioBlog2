[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(LesioBlog2.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(LesioBlog2.App_Start.NinjectWebCommon), "Stop")]

namespace LesioBlog2.App_Start
{
    using LesioBlog2_Repo.Abstract;
    using LesioBlog2_Repo.Concrete;
    using LesioBlog2_Repo.Models.Context;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System;
    using System.Web;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();


                kernel.Bind<IBlogContext>().To<BlogContext>();

                kernel.Bind<IPostRepo>().To<PostRepo>();

                kernel.Bind<IUserRepo>().To<UserRepo>();

                kernel.Bind<IGender>().To<GenderRepo>();

                kernel.Bind<ICommentRepo>().To<CommentRepo>();


                kernel.Bind<ITagRepo>().To<TagRepo>();

                kernel.Bind<ICodeRepo>().To<CodeRepo>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
        }        
    }
}



using aEMR.CommonViews.Assets.Resources;
using aEMR.CommonViews.Assets.Resources.PatientRegistration;
using aEMR.CommonViews.Assets.Resources.UserManagement;

namespace aEMR.CommonViews
{
    /// <summary>
    /// Wraps access to the strongly typed resource classes so that you can bind
    /// control properties to resource strings in XAML
    /// </summary>
    public sealed class ResourceWrapper
    {
        private static ApplicationStrings applicationStrings = new ApplicationStrings();
        private static SecurityQuestions securityQuestions = new SecurityQuestions();
        private static PatientRegistrationResources patientRegistrationResources = new PatientRegistrationResources();
        private static UserManagementResources userManagementResources = new UserManagementResources();
        /// <summary>
        /// Gets the <see cref="ApplicationStrings"/>.
        /// </summary>
        public ApplicationStrings ApplicationStrings
        {
            get { return applicationStrings; }
        }

        /// <summary>
        /// Gets the <see cref="SecurityQuestions"/>.
        /// </summary>
        public SecurityQuestions SecurityQuestions
        {
            get { return securityQuestions; }
        }

        /// <summary>
        /// Gets the <see cref="PatientRegistrationResources"/>.
        /// </summary>
        public PatientRegistrationResources PatientRegistrationResources
        {
            get { return patientRegistrationResources; }
        }

        /// <summary>
        /// Gets the <see cref="UserManagementResources"/>.
        /// </summary>
        public UserManagementResources UserManagementResources
        {
            get { return userManagementResources; }
        }
    }
}

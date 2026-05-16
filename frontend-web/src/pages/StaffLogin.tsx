import { UserCog } from 'lucide-react';
import { PortalLoginPage } from '../components/PortalLoginPage';

const StaffLogin = () => (
  <PortalLoginPage
    config={{
      variant: 'staff',
      title: 'Staff Portal',
      subtitle: 'Sign in to manage daily operations',
      allowedRoles: ['Staff'],
      successRole: 'Staff',
      defaultRedirect: '/staff/dashboard',
      emailPlaceholder: 'staff@example.com',
      otherPortal: { prompt: 'Admin portal?', to: '/login', label: 'Admin login' },
      BrandIcon: UserCog,
    }}
  />
);

export default StaffLogin;

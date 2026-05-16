import { Leaf } from 'lucide-react';
import { PortalLoginPage } from '../components/PortalLoginPage';

const Login = () => (
  <PortalLoginPage
    config={{
      variant: 'admin',
      title: 'Admin Portal',
      subtitle: 'Sign in to manage the system',
      allowedRoles: ['Admin'],
      successRole: 'Admin',
      defaultRedirect: '/admin/dashboard',
      emailPlaceholder: 'admin@example.com',
      otherPortal: { prompt: 'Staff portal?', to: '/staff/login', label: 'Staff login' },
      BrandIcon: Leaf,
    }}
  />
);

export default Login;

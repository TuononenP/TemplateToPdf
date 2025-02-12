import { Layout as ReactAdminLayout } from 'react-admin';
import { Menu } from './Menu';

export const Layout = (props: any) => (
    <ReactAdminLayout
        {...props}
        menu={Menu}
    />
); 
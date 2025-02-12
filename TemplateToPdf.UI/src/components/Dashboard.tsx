import { Card, CardContent, CardHeader } from '@mui/material';
import { Title } from 'react-admin';

const Dashboard = () => (
    <Card>
        <Title title="Dashboard" />
        <CardHeader title="Welcome to Template to PDF" />
        <CardContent>
            <p>
                This application allows you to manage HTML templates and generate PDFs from them.
                Use the navigation menu to:
            </p>
            <ul>
                <li>Create new templates</li>
                <li>Edit existing templates</li>
                <li>Generate PDFs from templates</li>
                <li>Manage your template library</li>
            </ul>
        </CardContent>
    </Card>
);

export default Dashboard; 
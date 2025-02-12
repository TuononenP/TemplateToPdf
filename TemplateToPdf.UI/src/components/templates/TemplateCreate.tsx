import {
    Create,
    SimpleForm,
    TextInput,
    required,
    TopToolbar,
    Button
} from 'react-admin';
import { Link } from 'react-router-dom';
import ArrowBack from '@mui/icons-material/ArrowBack';

const CreateActions = () => (
    <TopToolbar>
        <Button
            component={Link}
            to="/templates"
            label="Back"
            startIcon={<ArrowBack />}
        />
    </TopToolbar>
);

export const TemplateCreate = () => (
    <Create actions={<CreateActions />}>
        <SimpleForm>
            <TextInput source="name" validate={[required()]} fullWidth />
            <TextInput
                source="content"
                validate={[required()]}
                multiline
                rows={20}
                fullWidth
                defaultValue={`<html>
    <head>
        <style>
            body { font-family: Arial, sans-serif; padding: 20px; }
            .customer-info { margin: 20px 0; }
            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            th, td { padding: 8px; text-align: left; border: 1px solid #ddd; }
            th { background-color: #f5f5f5; font-weight: bold; }
            .total { font-weight: bold; background-color: #f9f9f9; }
            h1, h2 { margin-bottom: 10px; }
            .price { text-align: right; }
            .quantity { text-align: center; }
        </style>
    </head>
    <body>
        <h1>{{title}}</h1>
        <div>{{content}}</div>
    </body>
</html>`}
            />
        </SimpleForm>
    </Create>
); 
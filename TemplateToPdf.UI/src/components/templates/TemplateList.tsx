import {
    List,
    Datagrid,
    TextField,
    DateField,
    EditButton,
    DeleteButton,
    TextInput,
    CreateButton,
    TopToolbar,
    useRecordContext
} from 'react-admin';

const ListActions = () => (
    <TopToolbar>
        <CreateButton />
    </TopToolbar>
);

const TemplateFilters = [
    <TextInput source="name" label="Search by name" alwaysOn />
];

const DeleteWithConfirmation = () => {
    const record = useRecordContext();
    return (
        <DeleteButton
            mutationMode="pessimistic"
            confirmTitle={`Delete template "${record?.name}"`}
            confirmContent="Are you sure you want to delete this template?"
        />
    );
};

export const TemplateList = () => (
    <List
        actions={<ListActions />}
        filters={TemplateFilters}
        sort={{ field: 'updatedAt', order: 'DESC' }}
    >
        <Datagrid>
            <TextField source="id" />
            <TextField source="name" label="Name" />
            <DateField source="createdAt" showTime label="Created" />
            <DateField source="updatedAt" showTime label="Updated" />
            <EditButton />
            <DeleteWithConfirmation />
        </Datagrid>
    </List>
); 
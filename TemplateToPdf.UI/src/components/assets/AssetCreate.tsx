import {
    Create,
    SimpleForm,
    TextInput,
    SelectInput,
    required,
    TopToolbar,
    Button,
    useTranslate,
    useInput,
    useNotify,
    useRedirect,
} from 'react-admin';
import { useFormContext } from 'react-hook-form';
import { Link } from 'react-router-dom';
import ArrowBack from '@mui/icons-material/ArrowBack';
import AceEditor from 'react-ace';
import { Box, Typography, Paper } from '@mui/material';
import { useCallback } from 'react';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { AssetType } from 'types';

// Import ace editor themes and modes
import 'ace-builds/webpack-resolver';
import 'ace-builds/src-min-noconflict/ext-language_tools';
import 'ace-builds/src-min-noconflict/mode-css';
import 'ace-builds/src-min-noconflict/mode-handlebars';
import 'ace-builds/src-min-noconflict/theme-twilight';
import 'ace-builds/src-min-noconflict/theme-xcode';

const CreateActions = () => (
    <TopToolbar>
        <Button
            component={Link}
            to="/assets"
            label="Back"
            startIcon={<ArrowBack />}
        />
    </TopToolbar>
);

const ContentInput = ({ source, ...rest }: any) => {
    const translate = useTranslate();
    const theme = localStorage.getItem('theme') || 'light';
    const form = useFormContext();
    const type = parseInt(form.watch('type'));

    const {
        field: { value, onChange, onBlur }
    } = useInput({
        source,
        defaultValue: '',
        validate: [required()]
    });
    
    if (type === AssetType.Image || type === AssetType.Font) {
        return null;
    }
    
    return (
        <Box mt={2}>
            <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                {translate('assets.fields.content')} *
            </Box>
            <AceEditor
                mode="handlebars"
                theme={theme === 'dark' ? 'twilight' : 'xcode'}
                name={source}
                width="1000px"
                height="500px"
                fontSize={14}
                showPrintMargin={false}
                showGutter={true}
                highlightActiveLine={true}
                value={value || ''}
                onChange={(newValue) => {
                    onChange(newValue);
                }}
                onLoad={(editor) => {
                    editor.session.setUseWrapMode(true);
                }}
                onBlur={onBlur}
                setOptions={{
                    enableBasicAutocompletion: true,
                    enableLiveAutocompletion: true,
                    enableSnippets: true,
                    showLineNumbers: true,
                    tabSize: 4,
                    useWorker: false
                }}
            />
        </Box>
    );
};

const BinaryContentInput = ({ source, ...rest }: any) => {
    const translate = useTranslate();
    const notify = useNotify();
    const form = useFormContext();
    const type = parseInt(form.watch('type'));
    
    const {
        field: { value, onChange }
    } = useInput({
        source,
        validate: type === AssetType.Image || type === AssetType.Font ? [required()] : undefined
    });

    const handleFileChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            // Check file size (10MB limit)
            if (file.size > 10 * 1024 * 1024) {
                notify('File size exceeds 10MB limit', { type: 'error' });
                return;
            }

            // Set mimeType automatically for images
            if (type === AssetType.Image) {
                form.setValue('mimeType', file.type);
            }

            const reader = new FileReader();
            reader.onload = () => {
                const base64 = (reader.result as string).split(',')[1];
                onChange(base64);
            };
            reader.readAsDataURL(file);
        }
    }, [onChange, notify, type, form]);

    const handleDrop = useCallback((event: React.DragEvent<HTMLDivElement>) => {
        event.preventDefault();
        event.stopPropagation();
        
        const file = event.dataTransfer.files?.[0];
        if (file) {
            // Check file size (10MB limit)
            if (file.size > 10 * 1024 * 1024) {
                notify('File size exceeds 10MB limit', { type: 'error' });
                return;
            }

            // Set mimeType automatically for images
            if (type === AssetType.Image) {
                form.setValue('mimeType', file.type);
            }

            const reader = new FileReader();
            reader.onload = () => {
                const base64 = (reader.result as string).split(',')[1];
                onChange(base64);
            };
            reader.readAsDataURL(file);
        }
    }, [onChange, notify, type, form]);

    const handleDragOver = useCallback((event: React.DragEvent<HTMLDivElement>) => {
        event.preventDefault();
        event.stopPropagation();
    }, []);

    if (type !== AssetType.Image && type !== AssetType.Font) {
        return null;
    }

    return (
        <Box mt={2}>
            <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                {translate('assets.fields.binaryContent')} *
            </Box>
            <Paper
                variant="outlined"
                sx={{
                    p: 2,
                    textAlign: 'center',
                    cursor: 'pointer',
                    bgcolor: theme => theme.palette.mode === 'dark' ? 'rgba(255, 255, 255, 0.05)' : 'rgba(0, 0, 0, 0.02)',
                    '&:hover': {
                        bgcolor: theme => theme.palette.mode === 'dark' ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.05)',
                    }
                }}
                onDrop={handleDrop}
                onDragOver={handleDragOver}
                onClick={() => document.getElementById('file-input')?.click()}
            >
                <input
                    id="file-input"
                    type="file"
                    accept={type === AssetType.Image ? 'image/*' : '.woff2,.ttf,.otf'}
                    onChange={handleFileChange}
                    style={{ display: 'none' }}
                />
                <CloudUploadIcon sx={{ fontSize: 48, mb: 1, color: 'primary.main' }} />
                <Typography variant="body1" gutterBottom>
                    {translate('ra.input.file.upload_single')}
                </Typography>
                <Typography variant="body2" color="textSecondary">
                    {type === AssetType.Image ? 'Supported formats: JPG, PNG, GIF, WebP' : 'Supported formats: WOFF2, TTF, OTF'}
                </Typography>
                <Typography variant="body2" color="textSecondary">
                    Maximum size: 10MB
                </Typography>
            </Paper>
            {value && (
                <Box mt={2}>
                    {type === AssetType.Image && (
                        <img
                            src={`data:${form.getValues('mimeType')};base64,${value}`}
                            alt="Preview"
                            style={{ maxWidth: '100%', maxHeight: '300px', objectFit: 'contain' }}
                        />
                    )}
                    {type === AssetType.Font && (
                        <Typography color="success.main">
                            {translate('assets.create.fontUploaded')}
                        </Typography>
                    )}
                </Box>
            )}
        </Box>
    );
};

const generateReferenceName = (name: string): string => {
    return name
        .toLowerCase()
        .replace(/[^a-z0-9-]/g, '-') // Replace any character that's not a lowercase letter, number, or hyphen with a hyphen
        .replace(/-+/g, '-') // Replace multiple consecutive hyphens with a single hyphen
        .replace(/^-+|-+$/g, ''); // Remove hyphens from the start and end
};

const NameInput = () => {
    const {
        field: { onChange: onReferenceNameChange }
    } = useInput({ source: 'referenceName' });

    const handleNameChange = (event: any) => {
        const name = event.target.value;
        const newReferenceName = generateReferenceName(name);
        onReferenceNameChange(newReferenceName);
    };

    return (
        <TextInput 
            source="name" 
            validate={[required()]} 
            fullWidth 
            onChange={handleNameChange}
        />
    );
};

export const AssetCreate = () => {
    const validate = (values: any) => {
        const errors: any = {};
        
        if (!values.type) {
            errors.type = 'Required';
        }

        if (values.type === AssetType.Image || values.type === AssetType.Font) {
            if (!values.binaryContent) {
                errors.binaryContent = 'Required';
            }
        } else {
            if (!values.content) {
                errors.content = 'Required';
            }
        }

        return errors;
    };

    const transform = (data: any) => {
        // Convert type to number if it's a string
        if (typeof data.type === 'string') {
            data.type = parseInt(data.type, 10);
        }
        return data;
    };

    const notify = useNotify();
    const redirect = useRedirect();
    
    return (
        <Create 
            actions={<CreateActions />}
            transform={transform}
            mutationOptions={{
                onSuccess: () => {
                    notify('assets.notifications.created', { type: 'success' });
                    redirect('list', 'assets');
                },
                onError: () => {
                    notify('assets.notifications.error.create', { type: 'error' });
                }
            }}
        >
            <SimpleForm validate={validate}>
                <NameInput />
                <SelectInput
                    source="type"
                    validate={[required()]}
                    choices={[
                        { id: AssetType.Image, name: 'Image' },
                        { id: AssetType.Css, name: 'CSS' },
                        { id: AssetType.Font, name: 'Font' },
                        { id: AssetType.PartialTemplate, name: 'Partial Template' },
                    ]}
                    fullWidth
                />
                <TextInput source="description" fullWidth multiline />
                <ContentInput source="content" />
                <BinaryContentInput source="binaryContent" />
            </SimpleForm>
        </Create>
    );
}; 
-- Create the login
CREATE LOGIN pdftemplate WITH PASSWORD = 'pdf!creator-5841';

-- Create the user in the TemplateToPdf database
USE TemplateToPdf;
CREATE USER pdftemplate FOR LOGIN pdftemplate;

-- Grant necessary permissions to the user
ALTER ROLE db_owner ADD MEMBER pdftemplate;
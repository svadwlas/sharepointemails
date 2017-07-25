# SharePoint Emails
The project is intended to enrich SharePoint 2010 alert infrastructure to allow users to work with SharePoint items more productively via e-mail messages.

The main part of the project is custom alert templates infrastructure which allows to generate compelling e-mail messages for SharePoint alerts. Features:
1. Allows to create highly customizable XSLT templates for alert e-mail messages.
2. Custom XML nodes and tokens are added to default XSLT schema in order to support actual values insertion and custom activities (e. g. 3. item properties, site properties, item approval, deletion etc).
4. Alert template targets are Lists, Sites, Content types.
5. Allows to create template not only for message body but also for message subject to dynamically insert values.
6. Allows to specify e-mail "From" address.

Another valuable part is custom Discussion Board e-mail processor to solve following tasks:
1. More reliable replies sequence tracking than with default one. This feature in conjunction with custom alert templates could allow to receive complete message history from Discussion Board and post replies to right posts.
2. Allow users to work with discussion boards only via e-mail messages.

So as a result site administrators receive alerts look and feel flexibility and users get ability to perform most of work with SharePoint items just with e-mail messages. 

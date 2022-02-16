# Lockstep Example Connector

This program demonstrates how to create a connector that imports data from a financial or ERP system into the Lockstep Platform.  This connector demonstrates a few stages of working with financial and accounting data:

* Retrieve data from a source system

[FileDownloader](https://github.com/Lockstep-Network/lockstep-sdk-examples/blob/main/CsharpExample/ConnectorExample/FileDownloader.cs) demonstrates the ability to retrieve XML files from a [SSH FTP site](https://en.wikipedia.org/wiki/SSH_File_Transfer_Protocol).

* Map data from the source format to the Lockstep Platform format

[ModelConverter](https://github.com/Lockstep-Network/lockstep-sdk-examples/blob/main/CsharpExample/ConnectorExample/ModelConverter.cs) loads data in as XmlDocument objects, and provides a placeholder function for converting between source and destination formats.

* Call the Lockstep Upload Batch API

[BatchSubmitter](https://github.com/Lockstep-Network/lockstep-sdk-examples/blob/main/CsharpExample/ConnectorExample/BatchSubmitter.cs) writes all records to a CSV file, then merges all data into a ZIP file and submits it to the [Upload Sync File API](https://developer.lockstep.io/reference/post_api-v1-sync-zip).  This file is then placed into a queue for batch processing.

Finally, the code demonstrates how to check on the status of a sync request using [Retrieve Sync API](https://developer.lockstep.io/reference/get_api-v1-sync-id).

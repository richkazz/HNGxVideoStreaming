`VideoUploadController` API:

```markdown
# HNGxVideoStreaming

HNGxVideoStreaming is a video upload, transcription, and streaming service designed to simplify the process of uploading, transcribing, and streaming video content.

## Table of Contents

- [Features](#features)
- [Base URL](#base-url)
- [Endpoints](#endpoints)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Error Handling](#error-handling)
- [Contributing](#contributing)
- [License](#license)

## Features

- Start video uploads.
- Upload video chunks for ongoing uploads.
- Complete video uploads by merging chunks and extracting audio.
- Stream uploaded videos.
- Delete uploaded videos.
- Get a list of all uploaded videos.
- Transcribe audio from videos.

## Base URL

The base URL for all API endpoints is `https://example.com/VideoUpload`. Replace `example.com` with your actual domain.

## Endpoints

The API provides the following endpoints:

- `/startUpload`: Start a new video upload.
- `/UploadChunks`: Upload video chunks for an ongoing upload.
- `/UploadComplete`: Complete a video upload.
- `/StreamVideo/{uploadKey}`: Stream a video.
- `/DeleteVideo`: Delete a video.
- `/get-all`: Get a list of all uploaded videos.

For detailed documentation of each endpoint, please refer to the [API Documentation](#api-documentation) section below.

## Getting Started

To get started with HNGxVideoStreaming, follow these steps:

1. Clone this repository to your local machine:

   ```bash
   git clone https://github.com/your-username/HNGxVideoStreaming.git
   ```

2. Configure your environment variables, database, and storage settings as needed.

3. Build and run the application.

4. Access the API at the base URL mentioned above.

## API Documentation

For detailed documentation of the API endpoints, refer to the [API Documentation](API_DOCUMENTATION.md) file.

## Error Handling

If an error occurs during API requests, the API will return an error response with details in the `ErrorMessage` field. Refer to the API documentation for specific error codes and descriptions.

```

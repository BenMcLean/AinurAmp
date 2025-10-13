# AinurAmp
**It is very early days on this project so it is not ready for public consumption.**

## Project Overview
A self-hosted web-based music streaming application designed for personal music collections with specific focus on cuesheet support (FLAC/cue, APE/cue, WAV/cue, WV formats) and gapless playback. The application treats cuesheets as "virtual folders" and provides seamless navigation without requiring any changes to existing music collection structure.

The name "AinurAmp" was inspired by the Ainulindalë: the music of creation from J.R.R. Tolkien's Silmarillion. This reflects the application's purpose of playing music collections, particularly fitting for an application that treats cuesheets as unified musical works rather than as fragmented tracks.

## Core Requirements

### Fundamental Principles
- **Read-only music collection**: No write access to music files, mounted as read-only volume
- **No music collection database**: File system is the source of truth - no collection scanning or indexing required
- **Real-time directory listing**: Every navigation request triggers fresh filesystem queries
- **Preserve existing structure**: Zero modifications to music collection organization
- **Gapless playback**: Concept albums and live albums should play continuously as they were intended, but still allow for showing the separate tracks in the UI with seeking as if they were separate tracks.

### Cuesheet Support (Critical Feature)
- Support FLAC/cue, APE/cue, WAV/cue, and WV (embedded cue) formats
- Treat cuesheets as "virtual folders" in navigation
- Parse cuesheet data to present individual tracks as navigable items
- Support seeking within cuesheet tracks
- Maintain gapless playback within cuesheet albums

### User Experience Goals
- Navigate cuesheets like folders: `/path/to/album/Album.cue/01`
- Support both cuesheet tracks and direct audio file access
- Gapless playback for cuesheet albums
- Standard web browser compatibility (no plugins required)

### Deployment Context
This application is designed to be self-hosted and exposed to the internet (e.g., behind Cloudflare's reverse proxy). Local network streaming is already well-served by existing applications like foobar2000 - this project exists specifically to enable internet-based access to a personal music library.

## Design Philosophy
Core Architectural Principle: Direct File System Access

AinurAmp treats the music collection as read-only and interacts directly with 
the file system rather than maintaining a database for file tracking. This means:

- The collection can be modified by other tools (Samba, SFTP, etc.) while AinurAmp is running
- No synchronization or "library scanning" is needed
- No risk of AinurAmp corrupting the collection (it only reads)
- No risk of file system changes breaking AinurAmp (it reads on-demand)
- Playlists reference files directly; if files are deleted, playlists break (this is acceptable)

This approach trades database-backed features (fast searching, metadata caching) for architectural simplicity and resilience. No database file tracking means no synchronization problems, no migration headaches, and no divergence between "what AinurAmp thinks exists" and "what actually exists."

### Target Scale: Personal/Small Group Self-Hosting
This project is designed around the self-hosting individual use case (1-10 users), not enterprise deployment. This isn't a philosophical objection to larger scale - it simply means development effort focuses on simplicity and the small-scale experience rather than optimizations for thousands of concurrent users.

### Your Files, Your Way
AinurAmp never requires your music collection to conform to any particular organization, naming convention, or tagging standard. However your files are structured, named, or tagged - AinurAmp just works.

Most music servers impose their worldview on your collection:
- "Music must be organized as Artist/Album/Track"
- "Files must have proper ID3 tags"
- "Tracks must be in separate files, not in combined formats like FLAC/cue."

AinurAmp's philosophy: Your content outlives any application. **Apps come and go (remember WinAmp?), but your music collection is forever.** AinurAmp adapts to your files, not the other way around. Browse by folder structure, play what you point to, done.

### Why No Existing Solution Fits
Existing music servers either:
- Require databases (Plex, Jellyfin) which add complexity and synchronization issues
- Demand specific file organization or metadata standards
- Lack proper multi-user authentication for safe internet exposure
- Don't embrace the direct-file-system model
- Don't support cuesheet formats like FLAC/cue

AinurAmp fills this niche: simple, read-only, database-free file tracking, internet-safe, multi-user capable, FLAC/cue compatible and completely agnostic about how you organize your files.

AinurAmp is music streaming for people who hate music streaming and still buy (and rip) physical CDs.

## Technical Architecture

### Technology Stack

#### Development
- **IDE**: Visual Studio Community
- **Version control**: Git

#### Backend
- **Framework**: ASP.NET Core 8 with minimal APIs
- **Language**: C# .NET Core
- **Database**: SQLite with Entity Framework Core (user accounts, playlists, bookmarks only)
- **Transcoding**: FFmpeg via FFMpegCore wrapper
- **Container**: Docker with Linux base image

#### Frontend
- **UI Framework**: Blazor with InteractiveAuto rendering mode
  - First visit: Blazor Server (immediate interactivity)
  - Subsequent visits: Blazor WebAssembly (cached, client-side)
- **Component Library**: Blazor.Bootstrap for component architecture
- **Theme**: BOOTSTRA.386 for retro computing aesthetic
- **Audio Playback**: HTML5 Audio API with JavaScript enhancements
- **Client Logic**: TypeScript preferred over JavaScript

#### Deployment
- **Container**: Docker container
- **Orchestration**: Docker Compose
- **Music Storage**: Read-only volume mount
- **User Data**: SQLite database in persistent volume

## Technical Considerations

### Case Sensitivity Handling
- Implement canonical path resolution with case-insensitive matching
- Always redirect to canonical casing for consistent URIs
- Handle cuesheet-to-audio file matching across different case patterns

### Error Handling
- **Malformed cuesheets**: Log error and throw appropriate exception
- **Missing audio files**: Return 404 with descriptive error message
- **Naming conflicts**: Files take precedence over directories (e.g., album.cue file vs album.cue/ folder)
- **Transcoding failures**: Fallback to original file or error response

### Performance Optimization
- Real-time directory scanning (no caching by design)
- Efficient cuesheet parsing with minimal memory allocation
- Streaming transcoding with chunked transfer encoding
- Minimal database queries for user data

### Security Considerations
- Path traversal protection (prevent access outside music directory)
- User authentication and session management
- Input validation for all file path parameters
- Rate limiting for streaming endpoints

## Setup Instructions
**It is very early days on this project so it is not ready for public consumption.**

### Prerequisites

- .NET 9.0 SDK
- Node.js (v18 or later) and npm

### 1. Clone the repository

```bash
git clone [your-repo-url]
cd AinurAmp
```

### 2. Install LibMan CLI (if not already installed)

```bash
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
```

### 3. Navigate to the main project folder

```bash
cd AinurAmp
```

### 4. Restore client-side libraries

```bash
libman restore
```

This downloads all client-side dependencies (BOOTSTRA.386, jQuery) defined in `libman.json` to `wwwroot/lib/`.

### 5. Install npm dependencies

```bash
npm install
```

This installs TypeScript dependencies (Howler.js, esbuild, type definitions) defined in `package.json`.

### 6. Build the project

```bash
cd ..
dotnet build
```

The build process automatically:
- Compiles TypeScript files using esbuild
- Bundles npm modules
- Outputs JavaScript to `wwwroot/js/`

### 7. Run the application

```bash
dotnet run --project AinurAmp
```

Navigate to `https://localhost:[port]` (check console output for the actual port).

## Development Notes

### Client-side Dependencies
- **LibMan** manages CSS/JS libraries (Bootstrap theme, jQuery) - not committed to source control
- **npm** manages TypeScript/JavaScript modules (Howler.js) - `node_modules/` not committed to source control
- **esbuild** bundles TypeScript and npm modules during build - outputs not committed to source control
- Always run both `libman restore` and `npm install` after cloning or pulling dependency changes

### TypeScript Development
- TypeScript source files are in `AinurAmp/TypeScript/`
- Compiled/bundled JavaScript outputs to `AinurAmp/wwwroot/js/`
- TypeScript compilation happens automatically during `dotnet build`
- Changes to `.ts` files require a rebuild to take effect

### Updating Dependencies
- Update LibMan libraries: `libman update [library-name]@[version]`
- Update npm packages: `npm update` or edit `package.json` and run `npm install`

## Docker Considerations

The build process is designed to work in Docker containers:
- All dependencies are restored via standard CLI tools (`dotnet`, `libman`, `npm`)
- No IDE-specific requirements
- Build output is self-contained in `wwwroot/`

For production Docker builds, your Dockerfile should include:
1. Node.js (for npm and esbuild)
2. .NET SDK (includes LibMan capability)
3. Run `libman restore` and `npm install` before `dotnet build`

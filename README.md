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

### 1. Clone the repository

```bash
git clone [your-repo-url]
cd AinurAmp
```

### 2. Install LibMan CLI (if not already installed)

```bash
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
```

### 3. Restore client-side libraries

```bash
cd AinurAmp
libman restore
```

This will download all client-side dependencies (BOOTSTRA.386, jQuery, Bootstrap 4) defined in `libman.json` to the `wwwroot/lib/` folder.

### 4. Run the application

```bash
dotnet run
```

Navigate to `https://localhost:[port]` (check console output for the actual port).

## Development Notes
- Client-side libraries are managed via LibMan and are **not** committed to source control
- Always run `libman restore` after cloning or pulling changes to `libman.json`
- To update a library: `libman update [library-name]@[version]`

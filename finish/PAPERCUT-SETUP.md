# Papercut SMTP Setup with Docker

## What is Papercut SMTP?

Papercut SMTP is a local email testing tool that captures emails sent from your application without actually sending them to real recipients. Perfect for development and testing!

## Setup Instructions

### Option 1: Using Docker (Recommended)

1. **Pull and run Papercut SMTP using Docker:**

```powershell
docker run -d --name papercut -p 25:25 -p 37408:37408 jijiechen/papercut:latest
```

This command:
- Runs Papercut SMTP in detached mode (`-d`)
- Names the container "papercut" (`--name papercut`)
- Maps port 25 (SMTP) from container to host (`-p 25:25`)
- Maps port 37408 (Web UI) from container to host (`-p 37408:37408`)
- Uses the latest Papercut image

2. **Access Papercut Web UI:**

Open your browser and navigate to:
```
http://localhost:37408
```

3. **View captured emails:**

All emails sent by your application will appear in the Papercut web interface.

### Option 2: Using Docker Compose

Create a `docker-compose.yml` file in your project root:

```yaml
version: '3.8'

services:
  papercut:
    image: jijiechen/papercut:latest
    container_name: papercut-smtp
    ports:
      - "25:25"      # SMTP port
      - "37408:37408" # Web UI port
    restart: unless-stopped
```

Start Papercut:
```powershell
docker-compose up -d
```

Stop Papercut:
```powershell
docker-compose down
```

### Verify Setup

1. **Check if container is running:**
```powershell
docker ps
```

You should see the papercut container in the list.

2. **Test SMTP connection:**
```powershell
Test-NetConnection -ComputerName localhost -Port 25
```

Should return: `TcpTestSucceeded : True`

## Application Configuration

The SecureTaskHub Web application is already configured to use Papercut SMTP. Settings are in `appsettings.json`:

```json
{
  "SmtpSettings": {
    "Host": "localhost",
    "Port": "25",
    "FromEmail": "noreply@securetaskhub.local",
    "FromName": "SecureTaskHub"
  }
}
```

## Testing Password Reset

1. **Start Papercut SMTP** (using Docker command above)

2. **Run the SecureTaskHub Web application:**
```powershell
cd SecureTaskHub.Web
dotnet run
```

3. **Navigate to the application** (e.g., https://localhost:7xxx)

4. **Click "Forgot your password?" on the login page**

5. **Enter a user email** (e.g., `admin@demo.local`)

6. **Check Papercut web UI** (http://localhost:37408) - you should see the password reset email!

7. **Click the reset link in the email** to reset the password

## Demo User Accounts

Try password reset with these accounts:

- **admin@demo.local**
- **alice@demo.local**
- **bob@demo.local**

## Troubleshooting

### Port 25 already in use

If port 25 is already in use by another service (like IIS SMTP), use a different port:

1. Update `docker run` command:
```powershell
docker run -d --name papercut -p 2525:25 -p 37408:37408 jijiechen/papercut:latest
```

2. Update `appsettings.json`:
```json
{
  "SmtpSettings": {
    "Host": "localhost",
    "Port": "2525",
    ...
  }
}
```

### Cannot connect to Docker

Make sure Docker Desktop is running:
```powershell
docker version
```

### Emails not appearing in Papercut

1. Check container logs:
```powershell
docker logs papercut
```

2. Verify application logs for email sending errors

3. Check firewall settings for port 25

## Stop/Remove Papercut

```powershell
# Stop the container
docker stop papercut

# Remove the container
docker rm papercut
```

## Alternative: Papercut Desktop Application

If you prefer not to use Docker, you can download the desktop version:

1. Download from: https://github.com/ChangemakerStudios/Papercut-SMTP/releases
2. Install and run the application
3. It will automatically listen on port 25
4. View emails in the desktop application

---

**Note:** Papercut is for development only. Never use it in production!

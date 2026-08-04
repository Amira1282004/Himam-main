# 🔐 HTTPS Setup Guide - Himam Website

## 📋 Overview
This guide explains how to enable HTTPS with SSL/TLS certificates for production hosting, including:

- ✅ HTTPS enforcement for main domain and all subdomains
- ✅ Prevention of Mixed Content (HTTP resources on HTTPS pages)
- ✅ Automatic HTTP to HTTPS redirection
- ✅ HSTS (HTTP Strict Transport Security) with subdomain inclusion

---

## 🚀 Step-by-Step Implementation

### Step 1: Code Configuration (Already Implemented) ✅

The following code changes have been applied to enforce HTTPS:

#### **SecurityHeadersMiddleware.cs**
```csharp
// Upgrade Insecure Requests - Forces browsers to upgrade HTTP to HTTPS
context.Response.Headers.Append("Upgrade-Insecure-Requests", "1");

// Block all mixed content (HTTP on HTTPS)
context.Response.Headers.Append("Content-Security-Policy",
    "block-all-mixed-content;");

// HSTS with subdomain inclusion (only on HTTPS)
if (context.Request.IsHttps)
{
    context.Response.Headers.Append("Strict-Transport-Security",
        "max-age=31536000; includeSubDomains; preload");
}
```

#### **Program.cs**
```csharp
// Cookie security - Always require HTTPS
options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

// Force HTTP to HTTPS redirection
app.UseHttpsRedirection();
```

---

### Step 2: SSL Certificate Installation (Manual - Required) 🔧

#### **Option A: Let's Encrypt (Free & Recommended)**

**Prerequisites:**
- Domain name already purchased and pointing to your server
- Server access (SSH/RDP)
- Administrative privileges

**Steps:**

1. **Install Certbot (for Linux/Apache/Nginx):**
   ```bash
   sudo apt-get update
   sudo apt-get install certbot python3-certbot-apache
   # OR for Nginx:
   sudo apt-get install certbot python3-certbot-nginx
   ```

2. **Obtain Certificate:**
   ```bash
   sudo certbot --apache -d yourdomain.com -d www.yourdomain.com
   # OR for Nginx:
   sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com
   ```

3. **For Subdomains:**
   ```bash
   sudo certbot --apache -d yourdomain.com -d www.yourdomain.com -d admin.yourdomain.com -d api.yourdomain.com
   ```

4. **Auto-renewal (Certbot does this automatically):**
   ```bash
   sudo certbot renew --dry-run
   ```

#### **Option B: Commercial SSL Certificate**

**Steps:**

1. **Purchase SSL Certificate** from:
   - DigiCert
   - Comodo (Sectigo)
   - GlobalSign
   - Namecheap
   - GoDaddy

2. **Generate CSR (Certificate Signing Request):**
   ```bash
   # For Apache
   sudo openssl req -new -newkey rsa:2048 -nodes -keyout yourdomain.key -out yourdomain.csr
   ```

3. **Submit CSR to Certificate Authority** and download certificates

4. **Install Certificate** on your server:
   - Apache: Edit `ssl.conf` or `default-ssl.conf`
   - Nginx: Edit server block configuration
   - IIS: Import certificate via Server Certificates

#### **Option C: Cloudflare SSL (Easiest for Cloud Hosting)**

**Steps:**

1. **Sign up for Cloudflare** (Free tier available)

2. **Add your domain** to Cloudflare

3. **Update nameservers** at your domain registrar to Cloudflare's nameservers

4. **Enable SSL/TLS** in Cloudflare dashboard:
   - Mode: **Full (strict)** - recommended
   - Or: **Full** - if using self-signed certificate on origin

5. **Enable "Always Use HTTPS"** in Cloudflare:
   - Go to SSL/TLS → Edge Certificates
   - Turn on "Always Use HTTPS"
   - Turn on "Automatic HTTPS Rewrites"

---

### Step 3: Server Configuration

#### **For Apache**

Edit your Apache configuration file (`/etc/apache2/sites-available/yourdomain.conf`):

```apache
<VirtualHost *:80>
    ServerName yourdomain.com
    ServerAlias www.yourdomain.com
    Redirect permanent / https://yourdomain.com/
</VirtualHost>

<VirtualHost *:443>
    ServerName yourdomain.com
    ServerAlias www.yourdomain.com
    
    DocumentRoot /var/www/yourdomain
    
    SSLEngine on
    SSLCertificateFile /etc/letsencrypt/live/yourdomain.com/fullchain.pem
    SSLCertificateKeyFile /etc/letsencrypt/live/yourdomain.com/privkey.pem
    SSLCertificateChainFile /etc/letsencrypt/live/yourdomain.com/chain.pem
    
    # HSTS Header (optional, already in middleware)
    Header always set Strict-Transport-Security "max-age=31536000; includeSubDomains; preload"
    
    # Security Headers (optional, already in middleware)
    Header always set X-Frame-Options "DENY"
    Header always set X-Content-Type-Options "nosniff"
    
    <Directory /var/www/yourdomain>
        AllowOverride All
        Require all granted
    </Directory>
</VirtualHost>
```

Enable SSL module and restart Apache:
```bash
sudo a2enmod ssl
sudo a2enmod headers
sudo systemctl restart apache2
```

#### **For Nginx**

Edit your Nginx configuration file (`/etc/nginx/sites-available/yourdomain`):

```nginx
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name yourdomain.com www.yourdomain.com;

    ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;
    
    # SSL Configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
    
    # HSTS (optional, already in middleware)
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains; preload" always;
    
    root /var/www/yourdomain;
    index index.html;
    
    location / {
        proxy_pass http://localhost:5011;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Test and restart Nginx:
```bash
sudo nginx -t
sudo systemctl restart nginx
```

#### **For IIS (Windows)**

1. **Import SSL Certificate:**
   - Open IIS Manager
   - Go to Server Certificates
   - Click "Import"
   - Select your .pfx file
   - Enter password

2. **Configure HTTPS Binding:**
   - Select your site
   - Click "Bindings"
   - Add binding:
     - Type: https
     - IP address: All Unassigned
     - Port: 443
     - SSL certificate: Select your certificate

3. **Enable HSTS (IIS 10+):**
   - Install URL Rewrite Module
   - Add outbound rule:
     ```xml
     <system.webServer>
       <httpProtocol>
         <customHeaders>
           <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains; preload" />
         </customHeaders>
       </httpProtocol>
     </system.webServer>
     ```

4. **HTTP to HTTPS Redirect:**
   - Add URL Rewrite rule:
     ```xml
     <rewrite>
       <rules>
         <rule name="HTTP to HTTPS redirect" stopProcessing="true">
           <match url="(.*)" />
           <conditions>
             <add input="{HTTPS}" pattern="off" ignoreCase="true" />
           </conditions>
           <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Permanent" />
         </rule>
       </rules>
     </rewrite>
     ```

---

### Step 4: Configure ASP.NET Core for HTTPS

#### **appsettings.json (Production)**
```json
{
  "Urls": "https://localhost:5011;http://localhost:5010",
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:5011",
        "Certificate": {
          "Path": "/path/to/certificate.pfx",
          "Password": "your-certificate-password"
        }
      }
    }
  }
}
```

#### **Or Use Kestrel Configuration in Program.cs**
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5010); // HTTP
    options.ListenAnyIP(5011, configure => 
    {
        configure.UseHttps("path/to/certificate.pfx", "password");
    });
});
```

---

### Step 5: Verify HTTPS Configuration

#### **Test Tools:**

1. **SSL Labs Test:**
   - Visit: https://www.ssllabs.com/ssltest/
   - Enter your domain
   - Should get A+ rating

2. **Browser Test:**
   - Visit: https://yourdomain.com
   - Check for padlock icon
   - Click padlock → "Connection is secure"

3. **Mixed Content Test:**
   - Open Developer Tools (F12)
   - Go to Console tab
   - Look for "Mixed Content" warnings (should be none)

4. **HSTS Test:**
   - Visit: https://hstspreload.org/
   - Check if your domain can be preloaded

---

## 📝 Manual Steps Required (User Action)

### ✅ **Must Do:**

1. **Purchase Domain Name** (if not already owned)
   - From: Namecheap, GoDaddy, Google Domains, etc.

2. **Point Domain to Server**
   - Update DNS A record to your server IP
   - Wait for DNS propagation (24-48 hours)

3. **Obtain SSL Certificate**
   - Let's Encrypt (Free) - recommended
   - Or purchase commercial certificate

4. **Install SSL Certificate on Server**
   - Follow server-specific instructions above
   - This requires server access

5. **Configure Server for HTTPS**
   - Apache/Nginx/IIS configuration
   - Enable HTTP to HTTPS redirect

6. **Test HTTPS**
   - Verify SSL certificate is valid
   - Test HTTP to HTTPS redirect
   - Check for mixed content warnings

### ⚠️ **Optional but Recommended:**

1. **Cloudflare Setup** (if using Cloudflare)
   - Enable SSL/TLS
   - Enable "Always Use HTTPS"
   - Enable "Automatic HTTPS Rewrites"

2. **HSTS Preload Submission**
   - After 1 year of stable HTTPS
   - Submit to: https://hstspreload.org/

3. **SSL Monitoring**
   - Set up certificate expiry alerts
   - Use Uptime monitoring with SSL checks

---

## 🔍 Troubleshooting

### **Issue: "Your connection is not private"**
- **Cause**: Self-signed certificate or certificate not trusted
- **Solution**: Use certificate from trusted CA (Let's Encrypt or commercial)

### **Issue: Mixed Content Warnings**
- **Cause**: HTTP resources on HTTPS page
- **Solution**: 
  - Update all resource URLs to HTTPS
  - CSP `block-all-mixed-content` already enabled

### **Issue: HTTP not redirecting to HTTPS**
- **Cause**: Server not configured for redirect
- **Solution**: Configure server redirect (see Step 3)

### **Issue: HSTS not working**
- **Cause**: HSTS header only sent on HTTPS
- **Solution**: Ensure HTTPS is working first, then HSTS will activate

---

## 📊 Security Headers Verification

After setup, verify these headers are present:

```
Strict-Transport-Security: max-age=31536000; includeSubDomains; preload
Upgrade-Insecure-Requests: 1
Content-Security-Policy: block-all-mixed-content
X-Frame-Options: DENY
X-Content-Type-Options: nosniff
```

Use https://securityheaders.com/ to test.

---

## ✅ Checklist

- [ ] Domain purchased and DNS configured
- [ ] SSL certificate obtained
- [ ] SSL certificate installed on server
- [ ] Server configured for HTTPS
- [ ] HTTP to HTTPS redirect configured
- [ ] HSTS enabled
- [ ] Mixed content blocked
- [ ] SSL certificate tested
- [ ] Security headers verified
- [ ] Application tested on HTTPS
- [ ] Cookies set to Secure only
- [ ] Audit logs show HTTPS traffic

---

## 🎯 Summary

**Code Changes (Already Done):**
- ✅ Security headers with HSTS
- ✅ Mixed content blocking
- ✅ HTTPS upgrade requests
- ✅ Secure cookies only

**Manual Steps Required:**
- 🔧 Purchase domain
- 🔧 Point DNS to server
- 🔧 Obtain SSL certificate
- 🔧 Install SSL certificate
- 🔧 Configure server for HTTPS
- 🔧 Test and verify

**Estimated Time:**
- Let's Encrypt: 30 minutes
- Commercial SSL: 1-2 hours (including certificate issuance)
- Cloudflare: 15 minutes

---

## 📞 Support

If you encounter issues:
1. Check SSL Labs test results
2. Verify DNS propagation
3. Check server error logs
4. Test with different browsers
5. Use online SSL test tools

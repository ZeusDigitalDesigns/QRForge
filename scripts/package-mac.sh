#!/usr/bin/env bash
set -e

WORKSPACE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$WORKSPACE_DIR"

echo "=== Building QRForge macOS App for Thumb Drive ==="

DIST_DIR="$WORKSPACE_DIR/dist"
STAGING_DIR="$WORKSPACE_DIR/.staging"

rm -rf "$DIST_DIR" "$STAGING_DIR"
mkdir -p "$DIST_DIR" "$STAGING_DIR"

# 1. Create .icns from app.ico
echo "--> Generating macOS icon (.icns)..."
ICONSET_DIR="$STAGING_DIR/QRForge.iconset"
mkdir -p "$ICONSET_DIR"

python3 -c "
import os
from PIL import Image

img = Image.open('QRForge.UI/app.ico')
largest = max(img.ico.entry, key=lambda e: e.width)
img.seek(img.ico.entry.index(largest))
base = img.convert('RGBA')

iconset_path = '$ICONSET_DIR'
sizes = [
    (16, 'icon_16x16.png'),
    (32, 'icon_16x16@2x.png'),
    (32, 'icon_32x32.png'),
    (64, 'icon_32x32@2x.png'),
    (128, 'icon_128x128.png'),
    (256, 'icon_128x128@2x.png'),
    (256, 'icon_256x256.png'),
    (512, 'icon_256x256@2x.png'),
    (512, 'icon_512x512.png'),
    (1024, 'icon_512x512@2x.png')
]

for s, name in sizes:
    resized = base.resize((s, s), Image.Resampling.LANCZOS)
    resized.save(os.path.join(iconset_path, name))
"
iconutil -c icns "$ICONSET_DIR" -o "$STAGING_DIR/QRForge.icns"

# Helper function to package for a given RID
package_rid() {
    local RID="$1"
    local DISPLAY_NAME="$2"
    local DMG_NAME="$3"
    local PUBLISH_DIR="$STAGING_DIR/publish-$RID"
    local APP_BUNDLE="$STAGING_DIR/QRForge-$RID.app"
    local DMG_STAGING="$STAGING_DIR/dmg-$RID"

    echo "--> Publishing self-contained .NET app for $RID..."
    dotnet publish QRForge.UI/QRForge.UI.csproj \
        -c Release \
        -r "$RID" \
        --self-contained true \
        -o "$PUBLISH_DIR" \
        /p:DebugType=None \
        /p:DebugSymbols=false

    echo "--> Assembling $DISPLAY_NAME .app bundle..."
    mkdir -p "$APP_BUNDLE/Contents/MacOS"
    mkdir -p "$APP_BUNDLE/Contents/Resources"

    # Copy published files into Contents/MacOS
    cp -R "$PUBLISH_DIR"/* "$APP_BUNDLE/Contents/MacOS/"
    # Copy icon into Contents/Resources
    cp "$STAGING_DIR/QRForge.icns" "$APP_BUNDLE/Contents/Resources/QRForge.icns"

    # Create Info.plist
    cat << 'EOF' > "$APP_BUNDLE/Contents/Info.plist"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>QRForge</string>
    <key>CFBundleDisplayName</key>
    <string>QRForge</string>
    <key>CFBundleIdentifier</key>
    <string>com.qrforge.app</string>
    <key>CFBundleVersion</key>
    <string>1.0.0</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleSignature</key>
    <string>????</string>
    <key>CFBundleExecutable</key>
    <string>QRForge.UI</string>
    <key>CFBundleIconFile</key>
    <string>QRForge.icns</string>
    <key>NSPrincipalClass</key>
    <string>NSApplication</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>LSMinimumSystemVersion</key>
    <string>12.0</string>
</dict>
</plist>
EOF

    chmod +x "$APP_BUNDLE/Contents/MacOS/QRForge.UI"

    echo "--> Ad-hoc codesigning $DISPLAY_NAME .app..."
    codesign --force --deep --sign - "$APP_BUNDLE"

    # Create DMG staging directory with Drag-to-Applications symlink
    mkdir -p "$DMG_STAGING"
    cp -R "$APP_BUNDLE" "$DMG_STAGING/QRForge.app"
    ln -s /Applications "$DMG_STAGING/Applications"

    echo "--> Building disk image: $DMG_NAME..."
    hdiutil create -volname "QRForge" \
        -srcfolder "$DMG_STAGING" \
        -ov -format UDZO \
        "$DIST_DIR/$DMG_NAME"

    echo "--> Building portable ZIP: QRForge-$DISPLAY_NAME.zip..."
    ditto -c -k --sequesterRsrc --keepParent "$DMG_STAGING/QRForge.app" "$DIST_DIR/QRForge-$DISPLAY_NAME.zip"
}

# Package Apple Silicon (M1/M2/M3/M4)
package_rid "osx-arm64" "AppleSilicon" "QRForge-AppleSilicon.dmg"

# Copy the unpacked Apple Silicon app directly to dist for local testing or direct copy
cp -R "$STAGING_DIR/QRForge-osx-arm64.app" "$DIST_DIR/QRForge.app"

# Package Intel (x64)
package_rid "osx-x64" "Intel" "QRForge-Intel.dmg"

# Clean up staging
rm -rf "$STAGING_DIR"

echo "=== Packaging Complete! ==="
ls -lh "$DIST_DIR"


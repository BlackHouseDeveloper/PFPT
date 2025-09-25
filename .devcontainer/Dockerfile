# Use the official .NET 8 SDK image as base
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install the WASI experimental workload for Blazor/WebAssembly
RUN dotnet workload install wasi-experimental

# Install SQLite (common for MAUI/Blazor)
RUN apt-get update && \
    apt-get install -y \
        sqlite3 \
        libsqlite3-dev

# Install typical development tools and utilities
RUN apt-get update && \
    apt-get install -y \
        zsh \
        git \
        curl \
        wget \
        nano \
        vim \
        htop \
        tree \
        unzip \
        zip \
        build-essential \
        python3 \
        python3-pip \
        nodejs \
        npm \
        fonts-firacode \
        locales \
        openssh-client \
        ca-certificates \
        lldb \
        libssl-dev \
    && rm -rf /var/lib/apt/lists/*

# Set up locale (en_US.UTF-8, like macOS)
RUN locale-gen en_US.UTF-8
ENV LANG en_US.UTF-8
ENV LANGUAGE en_US:en
ENV LC_ALL=en_US.UTF-8

# Set zsh as default shell (like macOS)
RUN chsh -s /usr/bin/zsh

# Install Oh My Zsh for a familiar shell experience
RUN sh -c "$(curl -fsSL https://raw.githubusercontent.com/ohmyzsh/ohmyzsh/master/tools/install.sh)" || true

# Install Roslynator CLI globally
RUN dotnet tool install -g roslynator.dotnet.cli

# Install Entity Framework CLI globally (for migrations)
RUN dotnet tool install -g dotnet-ef --version 8.0.8

# Clean up
RUN apt-get clean

# Set working directory
WORKDIR /workspaces/PFPT

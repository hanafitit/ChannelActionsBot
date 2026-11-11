
# ── Build C# pinger ──
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS pinger-build

WORKDIR /src

COPY pinger/Pinger.csproj ./

RUN dotnet restore Pinger.csproj -r linux-x64

COPY pinger/ ./

RUN dotnet publish Pinger.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    --no-restore \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:InvariantGlobalization=true \
    -o /out


# ── Main application ──
FROM denoland/deno:2.1.4

WORKDIR /app

COPY deno.json ./
COPY . .

# Copy compiled C# executable
COPY --chmod=755 --from=pinger-build /out/Pinger /app/pinger/Pinger

RUN deno cache main.ts

ENV PORT=8000
ENV PING_URL=http://127.0.0.1:8000/
ENV PING_INTERVAL_SECONDS=30

EXPOSE 8000

# Start both processes
ENTRYPOINT ["/bin/sh", "-c"]
CMD ["/app/pinger/Pinger & exec deno task poll"]

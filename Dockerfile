
FROM denoland/deno:2.1.4

WORKDIR /app

COPY deno.json ./
COPY . .

RUN deno cache main.ts

ENV PORT=8000

EXPOSE 8000

CMD ["deno", "task", "poll"]

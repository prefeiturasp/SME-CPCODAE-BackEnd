FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 44343

# Cria o diretorio src e copiando todo o codigo-fonte para ele
WORKDIR /src
COPY . ./

# Acessa o diretorio src, publicando para o diretorio "publish"
WORKDIR /src
RUN dotnet publish "src/FIA.SME.Aquisicao.Api/FIA.SME.Aquisicao.Api.csproj" -c Release -o publish

# Define a imagem do runtime
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

RUN ["apt-get", "update"]
RUN apt-get install vim -y

# Adicionando bibliotecas para a cultura PT-BR
RUN apt-get install -y locales
RUN sed -i -e 's/# pt_BR.UTF-8 UTF-8/pt_BR.UTF-8 UTF-8/' /etc/locale.gen && \
    locale-gen

RUN echo "deb http://deb.debian.org/debian stable main contrib non-free" > /etc/apt/sources.list \
    && echo "ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true" | debconf-set-selections \
    && apt-get update \
    && apt-get install -y \
#        ttf-mscorefonts-installer \
        fonts-liberation \
    && apt-get clean \
    && apt-get autoremove -y \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copia todos os publicaveis para o diretorio app
COPY --from=build-env /src/publish .

RUN rm -rf publish cs de es fr it ja ko pl ru tr zh-* *.bat

# Define o environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS http://*:44343
ENV PORT = 44343;
ENV HOST = '0.0.0.0';

# Configuracao para utilizacao do horario padrao PT-BR
ENV TZ=America/Sao_Paulo
ENV LANG pt_BR.UTF-8
ENV LANGUAGE ${LANG}
ENV LC_ALL ${LANG}

ENTRYPOINT ["dotnet", "FIA.SME.Aquisicao.Api.dll"]
# 📷 Camera System

Asset para padronização e pré-configuração da câmera principal em jogos Unity.

---

## Como funciona

O sistema fornece uma câmera pré-configurada com definições comuns nos jogos desenvolvidos: um `CinemachineBrain` acoplado; consistência de resolução; pós-processamento, etc. 

## Como usar

1. Adicione o asset a, **pelo menos**, a primeira cena do jogo.
2. O objeto possui `DontDestroyOnLoad` e persistirá durante toda a sessão.
3. Acesse o sistema externamente via glue code ou conexão direta para utilizar os métodos disponibilizados.

## Pré-requisitos

- **Cinemachine** instalado no projeto.

## Configuração

As opções de configuração do sistema são definidas pelo Scriptable Object **`CameraData`**, localizado na pasta `_Data`.

### `AspectRatioHandler`

Script responsável por manter o aspect ratio da tela em um valor fixo, adicionando barras pretas nas laterais ou no topo/base para preservar a resolução padrão.

O valor do aspect ratio (padrão **16:9**) e a cor das barras podem ser alterados pelo `CameraData`.

### `CameraManager`

Script auxiliar responsável por aplicar o `DontDestroyOnLoad` ao objeto e expor o atributo `BlendTime` do `CinemachineBrain` (tempo de transição entre câmeras) para alteração externa.

Para modificar o `BlendTime` em tempo de execução, chame a função `SetBlendTime()` a partir de outro sistema, passando o novo valor desejado.
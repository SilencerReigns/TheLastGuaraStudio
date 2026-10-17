# 🔊 Audio System

Asset para padronização e pré-configuração de um sistema de áudio em jogos Unity.

---

## Como funciona

O sistema expõe métodos para reproduzir efeitos sonoros e músicas, sejam eles passados por parâmetro ou definidos internamente. Inclui também configuração de mixer de áudio e sliders para controle de volume por categoria.

## Como usar

1. Adicione o asset à **primeira cena do jogo** (ou à cena principal onde o jogo ocorre).
2. O objeto possui `DontDestroyOnLoad` e persistirá durante toda a sessão.
3. Acesse o sistema externamente via glue code ou conexão direta para utilizar os métodos disponibilizados.

## Configuração

As músicas padrão do jogo (ex: menu e gameplay) podem ser definidas pelo Scriptable Object **`AudioData`**, localizado na pasta `_Data`.

### `AudioManager`

Script principal do sistema. Expõe os métodos `PlayMusic()` e `PlaySFX()`, que podem ser chamados por outros sistemas para reproduzir uma música ou efeito sonoro específico.

Ambos aceitam um `AudioSource` como parâmetro opcional, permitindo sons posicionais 3D. O script também permite tocar as músicas padrão definidas no `AudioData`.

### `MixerController`

Script responsável por controlar os volumes do mixer e persistir as configurações entre sessões via `PlayerPrefs`.

### `SliderSetup`

Script para configurar os sliders de volume nos menus. Deve ser adicionado a cada slider, e a função `ChangeVolume()` é a que deve ser vinculada ao evento do slider.

Ao ser alterado, o slider emite um sinal C# com o novo valor e o `MixerController` responde, atualizando os atributos do mixer.
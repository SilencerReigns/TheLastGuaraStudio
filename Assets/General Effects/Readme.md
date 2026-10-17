# ✨ General Effects

Feature com múltiplos efeitos gerais que podem ser usados por diversos sistemas do jogo.

---

## Como funciona

O sistema fornece prefabs de efeitos pré-configurados. Qualquer sistema pode usá-los: basta adicionar os prefabs ao sistema e invocar as funções que iniciam e interrompem cada efeito.

## Como usar

1. Adicione o prefab do efeito desejado à hierarquia do sistema (comumente como filho do objeto que contém o **Effects Controller** daquele sistema).
2. Adicione a referência ao efeito no **Effects Controller** do sistema (ou em outro código responsável por tocar os efeitos).
3. Invoque as funções de início e fim do efeito quando o sistema desejar.

## Pré-requisitos

- **Cinemachine** instalado no projeto.
- **DOTween** instalado no projeto.

## Configuração

### `Default Data`

Scriptable Object que contém os valores padrão de todos os efeitos implementados.

Todo efeito aceita, opcionalmente, parâmetros nas suas funções. Se um parâmetro não for especificado por quem chamou a função, o efeito usa automaticamente o valor definido em **Default Data**.

> Alguns parâmetros, quando passados, alteram o comportamento do efeito. Nesses casos, o código está devidamente comentado.
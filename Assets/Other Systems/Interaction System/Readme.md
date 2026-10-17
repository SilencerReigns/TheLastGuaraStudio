# 🤝 Interaction System

Asset para fácil implementação de uma funcionalidade de interação ao input do jogador.

---

## Como funciona

O sistema fornece prefabs que podem ser adicionados como filhos a entidades do jogo e, com poucas configurações, permitir interação por input do jogador.

## Como usar

1. Adicione o prefab **Interaction Detector** como filho da entidade do jogador.
2. Adicione o prefab **Interaction Target** como filho de entidades ou objetos que devem ser interagíveis.
3. Configure os colliders e a posição do indicador visual para ficarem de acordo com a entidade.
4. Defina pelo evento do Interaction Target o que a interação irá fazer.
5. Conecte, via Glue Code ou conexão direta, o input do jogador ao Interaction Detector, chamando a função **Interact**

## Pré-requisitos

- Um PC que roda a Unity

## Configuração

O sistema não precisa de configuração alguma, só adicionar os prefabs e definir o que a(s) função(ões) que a interação irá invocar.
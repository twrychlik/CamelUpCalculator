<h1>Camel Up Betting Calculator</h1>

This application calculates the perfect bets to play in the game Camel Up: https://www.boardgamegeek.com/boardgame/153938/camel 

Rules: https://www.fgbradleys.com/rules/rules2/CamelUp-rules.pdf

![How to this calculator](/HowToUse.md)

<h2>Camel Up</h2>
In Camel Up, up to eight players bet on five racing camels racing around a pyramid, trying to guess (or calculate) which camels will win on each leg, and which camel will win overall. Over the course of a leg, each camel will have a die rolled (with values 1-3) and move forward that number of spaces. However, camels can sit on top of other camels forming a stack! This means that camels can get carried forward by others, making calculating probabilities quite unintuitive. To further complicate matters you can place tiles on the board to give your favourite camel and boost, or to try and hinder a camel an opponent has bet on.

<h2>The Calculator</h2>
This calculator gives the expected return on all possible moves:
<ul>
<li> Leg bet, worth 1, 2, 3 or 5 points</li>
 <li> End bet, worth 1, 2, 3 or 5 points</li>
 <li> Placing an oasis or mirage, worth 1 point per landed camel</li>
 <li> Rolling the dice, worth 1 point </li>
 </ul>
 The calculator will even take into account any bets you may have made, to better calculate your expected return on an oasis or mirage tile.


![Image of CamelUp](/CamelUp.PNG) <br>
Here we see the bet with best expectation is White to win the leg (if the bet is still available). Note that any bet with an expectation less than 1 is unwise, as rolling the dice gives a point. While uncommon, placing a mirage/oasis here on tile 11 is beneficial. 

<h2>Mathematics</h2>
There are 20!/15! = 1,860,480 postions the camels can be positioned in on a 16 tile board (including stacking). This reduces to 15504, when generalising out colours. This allows for complete calculation, given the game can last at most 16 legs.

In order to compute the probablility of a given camel winning the overall race, I initially calculated the first markov matrix **M**, where **M**<sub>ijk</sub> is the probability of reaching state _j_ from state _i_ with permutation _k_ after a single leg. From this we can simply square **M** several times **M**<sup>2<sup>2<sup>2</sup></sup></sup> to reach **M**<sup>16</sup>. Given the game must finish after at most 16 legs (not couting mirages), **M**<sup>16</sup> only holds non-zero values for _j_ = 15505 (an additional position, _j_ = 15505 is added to represent a win). The values of **M**<sub>i,15505,k</sub> therefore give the probability that the first camel in permutation _k_ will win for initial position _i_.

**M**16 has been precalculated and stored as a jagged matrix Matrix5.xml. Some optimisation was needed to make this calculation take less than 24 hours, as well as breaking the earlier matrices up in memory to avoid a 2gb memory limit. Reading these precalculated probabilities gives good performance overall, and well within a reasonable real life game turn.

From this, one can enumerate all positions a leg can end in, and from there, multiply that position probablility vector with **M**16 to give the overall ending position probability vector. 

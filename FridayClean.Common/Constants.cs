﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FridayClean.Common
{
	public static class Constants
	{
		public const string AuthHeaderName = "interceptor-access-token";
		public const string UserPhoneHeaderName = "interceptor-user-phone";
		public const string AccessTokenSecureStorageKey = "fridayclean-access-token";
		public const int PriceStep = 5;

		public static class Descriptions
		{
			public const string MaintenanceCleaning =
@"Поддерживающая уборка
	Комнаты и коридоры
		Моем стеклянные поверхности и зеркала
		Раскладываем вещи по местам
		Пылесосим и моем пол и плинтусы
		Застилаем кровать и меняем белье
		Протираем все доступные поверхности
		Пылесосим снаружи мягкую мебель
	Кухня
		Пылесосим снаружи мягкую мебель
		Протираем мебель снаружи
		Моем раковину и посуду
		Моем снаружи плиту, вытяжку и духовку
		Моем столешницу и кухонный фартук
		Выносим мусор
	Ванная комната
		Моем и обеззараживаем сантехнику
		Протираем полки и зеркала
		Очищаем плитку от локальных пятен
		Пылесосим коврики и моем пол
		Моем ванну и душевую кабину";
			public const string ComplexCleaning = @"
Комплексная уборка
	Комнаты и коридоры
		Раскладываем вещи на места
		Пылесосим ковры
		Моем пол и плинтусы
		Протираем все горизонтальные поверхности
		Застилаем кровать и меняем белье
	Кухня
		Моем снаружи холодильник, духовку
		Моем кухонную технику снаружи
		Моем столешницу
		Моем плиту и стену над ней
		Выносим мусор
		Моем раковину и посуду
	Ванная комната
		Протираем полки и зеркала на всю высоту
		Моем и обеззараживаем сантехнику
		Моем пол и плинтусы
		Пылесосим коврики";
			public const string GeneralCleaning = @"
Генеральная уборка
	Комнаты и коридоры
		Моем стеклянные поверхности и зеркала
		Удаляем пыль с бытовых приборов
		Удаляем пыль с потолков и стен
		Пылесосим и моем пол + плинтусы
		Моем светильники
		Протираем все доступные поверхности
		Отодвигаем мебель и моем за ней
		Моем мебель снаружи и внутри
		Пылесосим мягкую мебель
	Кухня
		Моем кухонную сантехнику снаружи и внутри
		Моем мебель снаружи и внутри
		Чистим парогенератором труднодоступные места
		Моем плиту и столешницу
		Выносим мусор
		Моем вытяжку, включая фильтры
		Моем раковину и посуду
	Ванная комната
		Протираем полки и зеркала
		Моем и обеззараживаем сантехнику
		Чистим парогенератором труднодоступные места
		Моем ванну, душевую кабину
		Моем потолок и стены на всю высоту";

		}
		public static class Messages
		{
			public const string UnableToCallRpcMessage =
				"Ошибка: Невозможно выполнить запрос. Возможно, сервер недоступен, попробуйте позже.";

			public const string SecureStorageNotSupportedMessage =
				"Ошибка: Ваш телефон не поддерживает SecureStorage! После перезапуска приложения придется авторизоваться заного.";
		}
	}
}
